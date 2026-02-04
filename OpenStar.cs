using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Http.Extensions;
using OpenStar.Cluster;
using OpenStar.Cluster.Config;
using OpenStar.Cluster.Loader;
using OpenStar.Endpoint;
using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar;

/// <summary>
/// OpenStar main class
///
/// This class internally is a Cluster as well, as it implements it's interface.
/// </summary>
public class OpenStar : ICluster
{
    /// <summary>
    /// The OpenStar Instance
    /// </summary>
    public static OpenStar Instance { get; private set; } = null!;

    /// <summary>
    /// OpenStar config instance
    /// </summary>
    public readonly OpenStarConfig Config;

    /// <summary>
    /// The path where OpenStar will write to
    /// </summary>
    public readonly string StoragePath = Path.Combine(AppContext.BaseDirectory, "OpenStarRoot");

    /// <summary>
    /// Holds and manages Clusters
    /// </summary>
    public readonly ClusterManager Manager = new ClusterManager();

    /// <summary>
    /// List of cluster loaders to use when loading Clusters
    /// </summary>
    private readonly ClusterLoader[] _loaders;

    /// <summary>
    /// The ASP.NET WebApplication
    /// </summary>
    public WebApplication? App { get; private set; }

    /// <summary>
    /// Default OpenStar logger
    /// </summary>
    public ILogger Logger { get; private set; }

    /// <summary>
    /// Creates a new OpenStar instance
    /// </summary>
    private OpenStar()
    {
        Logger = CreateLogger();

        Config = ClusterConfigFile.Load<OpenStarConfig>(this);

        string clp = Path.Combine(StoragePath, "Clusters");
        _loaders = [new FilesystemClusterLoader(this.Manager, clp)];
    }

    /// <inheritdoc />
    public string GetName() => "OpenStar";

    /// <inheritdoc />
    public string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    /// <inheritdoc />
    public string GetStorageDirectory() => StoragePath;

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
    public ClusterConfig GetConfig() => Config;

    /// <summary>
    /// Main function
    /// </summary>
    public static async Task Main()
    {
        Instance = new OpenStar();

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

    /// <summary>
    /// Starts OpenStar and the ASP.NET Application
    /// </summary>
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
            await c.Value.SetupApplication(App);
            Log.Information("Set up Cluster {cluster}", c.Value.GetName());
        }
    }


}