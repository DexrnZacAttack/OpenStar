using System.CommandLine;
using System.Reflection;
using OpenStar.Core.Cluster.Config;
using OpenStar.Client.Cluster.Loader;
using OpenStar.Client.Endpoint;
using OpenStar.Core;
using OpenStar.Core.Cluster;
using OpenStar.Core.Events;
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

    /// <inheritdoc />
    public ILogger Logger { get; }

    /// <summary>
    /// Holds and manages Clusters
    /// </summary>
    public ClusterManager Manager { get; } = new();

    /// <summary>
    /// List of cluster loaders to use when loading Clusters
    /// </summary>
    private readonly ClusterLoader[] _loaders;

    /// <inheritdoc />
    public WebApplication? App { get; set; }
    
    /// <inheritdoc />
    public string Name => "OpenStar";

    /// <inheritdoc />
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    
    /// <inheritdoc />
    public string StorageDirectory { get; }

    //backing field so we still have typed config access
    private readonly OpenStarConfig _config;

    /// <inheritdoc />
    public IClusterConfig Config => _config;

    /// <summary>
    /// Creates a new OpenStar instance
    /// </summary>
    private OpenStar(string storagePath)
    {
        this.StorageDirectory = storagePath;

        Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: Constants.ConsoleOutputTemplate)
                .WriteTo
                .File(Path.Combine(StorageDirectory, "logs", $"OpenStar-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log"),
                      retainedFileCountLimit: null,
                      outputTemplate: Constants.FileOutputTemplate)
                .MinimumLevel.Debug()
                .CreateLogger()
                .ForContext("SourceContext", typeof(OpenStar).Namespace);

        _config = ClusterConfigFile.Load<OpenStarConfig>(this);

        string clp = Path.Combine(StorageDirectory, "Clusters");
        _loaders = [new FilesystemClusterLoader(this.Manager, clp, Logger.ForContext<FilesystemClusterLoader>())];

        OpenStarEvents.InitializeWebApplication += SetupApplication;
        OpenStarEvents.InitializeWebApplicationBuilder += SetupApplicationBuilder;
    }
    
    private Task SetupApplication(WebApplication app)
    {
        app.Use(Middleware.Invoke);

        return Task.CompletedTask;
    }

    private Task SetupApplicationBuilder(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(Logger);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool IsDevelopmentEnvironment() => App?.Environment.IsDevelopment() ?? false;

    /// <summary>
    /// Main function
    /// </summary>
    public static async Task<int> Main(string[] args)
    {
        Option<string> storageDirectoryOption = new("--storage-directory")
        {
            Description = "The directory used to store webserver data",
            Required = false
        };

        storageDirectoryOption.Validators.Add(res =>
        {
            string? p = res.GetValue(storageDirectoryOption);
            if (p == null)
                return; // we'll use application dir instead

            if (!Directory.Exists(p))
                res.AddError($"Storage Directory '{p}' does not exist");
        });

        RootCommand rootCommand = new("Extensible ASP.NET host")
        {
            storageDirectoryOption
        };

        rootCommand.SetAction(async res =>
        {
            string storagePath = res.GetValue(storageDirectoryOption) ?? Path.Combine(AppContext.BaseDirectory, "OpenStarRoot");

            Instance = new OpenStar(storagePath);
            OpenStarCore.Instance = new OpenStarCore(Instance);

            foreach (ClusterLoader loader in Instance._loaders)
            {
                loader.Load();
                loader.Register();
            }
            await OpenStarEvents.OnClusterInitialize(Instance);

            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            await OpenStarEvents.OnWebApplicationBuilderInitialize(builder);

            Instance.App = builder.Build();
            await OpenStarEvents.OnWebApplicationInitialize(Instance.App);

            Instance.Start();
        });

        return await rootCommand.Parse(args).InvokeAsync();
    }

    /// <inheritdoc />
    public void Start()
    {
        Logger.Information("Starting OpenStar v{Version}", Version);

        if (App == null)
            throw new NullReferenceException("The ASP.NET App has not been set up yet, please initialize it first.");

        App.Run();
    }
}