using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using OpenStar.Core.Cluster.Config;
using OpenStar.Client.Cluster.Loader;
using OpenStar.Client.Endpoint;
using OpenStar.Core;
using OpenStar.Core.Cluster;
using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar.Client;

/// <summary>
/// OpenStar main class
///
/// This class internally is a Cluster as well, as it implements it's interface.
/// </summary>
public class OpenStar : IOpenStarClient
{
    /// <summary>
    /// The OpenStar Instance
    /// </summary>
    public static OpenStar Instance { get; private set; } = null!;

    /// <summary>
    /// OpenStar config instance
    /// </summary>
    private readonly OpenStarConfig _config;

    /// <summary>
    /// The path where OpenStar will write to
    /// </summary>
    private readonly string _storagePath = Path.Combine(AppContext.BaseDirectory, "OpenStarRoot");

    /// <inheritdoc />
    public ILogger Logger { get; }

    /// <summary>
    /// Holds and manages Clusters
    /// </summary>
    public ClusterManager Manager { get; } = new ClusterManager();

    /// <summary>
    /// List of cluster loaders to use when loading Clusters
    /// </summary>
    private readonly ClusterLoader[] _loaders;

    /// <inheritdoc />
    public WebApplication? App { get; set; }

    /// <summary>
    /// Creates a new OpenStar instance
    /// </summary>
    private OpenStar()
    {
        Logger = CreateLogger();

        _config = ClusterConfigFile.Load<OpenStarConfig>(this);

        string clp = Path.Combine(_storagePath, "Clusters");
        _loaders = [new FilesystemClusterLoader(this.Manager, clp, Logger.ForContext<FilesystemClusterLoader>())];
    }

    /// <inheritdoc />
    public string GetName() => "OpenStar";

    /// <inheritdoc />
    public string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    /// <inheritdoc />
    public string GetStorageDirectory() => _storagePath;

    /// <inheritdoc />
    public ILogger CreateLogger() =>
        new LoggerConfiguration()
           .WriteTo.Console(outputTemplate: Constants.ConsoleOutputTemplate)
           .WriteTo
           .File(Path.Combine(GetStorageDirectory(), "logs", $"OpenStar-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log"),
                 retainedFileCountLimit: null,
                 outputTemplate: Constants.FileOutputTemplate)
           .MinimumLevel.Debug()
           .CreateLogger()
           .ForContext("SourceContext", typeof(OpenStar).Namespace);

    /// <inheritdoc />
    public Task SetupApplication(WebApplication app)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SetupApplicationBuilder(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(Log.Logger);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public ClusterConfig GetConfig() => _config;

    /// <inheritdoc />
    public bool IsDevelopmentEnvironment() => App?.Environment.IsDevelopment() ?? false;

    /// <summary>
    /// Main function
    /// </summary>
    public static async Task Main()
    {
        Instance = new OpenStar();
        OpenStarCore.Instance = new OpenStarCore(Instance);

        foreach (ClusterLoader loader in Instance._loaders)
        {
            loader.Load();
            loader.Register();
        }
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        await Instance.InitBuilder(builder);

        Instance.App = builder.Build();

        await Instance.InitClusters();
        Instance.Start();
    }

    /// <inheritdoc />
    public void Start()
    {
        Log.Information("Starting OpenStar v{Version}", GetVersion());

        if (App == null)
            throw new NullReferenceException("The ASP.NET App has not been set up yet, please initialize it first.");

        App.Run();
    }

    /// <summary>
    /// Initializes a WebApplicationBuilder
    /// </summary>
    /// <param name="builder">A WebApplicationBuilder</param>
    private async Task InitBuilder(WebApplicationBuilder builder)
    {
        await SetupApplicationBuilder(builder);
        foreach (var c in Manager.Clusters)
        {
            c.Value.Logger.Information("Setting up WebApplicationBuilder");
            await c.Value.SetupApplicationBuilder(builder);
        }
    }

    /// <summary>
    /// Initializes all Clusters and our ASP.NET WebApplication
    /// </summary>
    private async Task InitClusters()
    {
        if (App == null)
            throw new NullReferenceException("The ASP.NET App has not been set up yet, please initialize it first.");

        App.Use(Middleware.Invoke);
        await SetupApplication(App);
        foreach (var c in Manager.Clusters)
        {
            c.Value.Logger.Information("Setting up WebApplication");
            await c.Value.SetupApplication(App);

            Logger.Information("Set up Cluster {cluster}", c.Value.GetName());
        }
    }


}