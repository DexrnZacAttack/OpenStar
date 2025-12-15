using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Http.Extensions;
using OpenStar.Cluster;
using OpenStar.Cluster.Config;
using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar;

public class OpenStar : ICluster
{
    private readonly Dictionary<string, AssemblyLoadContext> _asmContexts = new();
    private readonly Dictionary<Type, ICluster> _clusters = [];

    public readonly OpenStarConfig Config;
    public readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "OpenStarRoot");

    private OpenStar()
    {
        Log.Logger = CreateLogger();

        Config = ClusterConfigFile.Load<OpenStarConfig>(this);
    }

    public WebApplication App { get; private set; }
    public static OpenStar Instance { get; private set; } = null!;

    public string GetName() => "OpenStar";

    public string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    public string GetStorageDirectory() => ConfigPath;

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

    public Task SetupApplication(WebApplication app)
    {
        return Task.CompletedTask;
    }

    public Task SetupApplicationBuilder(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(Log.Logger);
        return Task.CompletedTask;
    }

    public ClusterConfig? GetConfig() => Config;

    public T? TryGetCluster<T>() where T : Cluster.Cluster
        => _clusters[typeof(T)] as T;

    public static async Task Main(string[] args)
    {
        Instance = new OpenStar();

        Instance.RegisterClusters();
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        await Instance.InitBuilder(builder);

        Instance.App = builder.Build();

        await Instance.InitClusters();
        Instance.Start();
    }

    public void Start()
    {
        Log.Information("Starting OpenStar v{Version}", GetVersion());
        App.Run();
    }

    private void RegisterClusters()
    {
        string clp = Path.Combine(ConfigPath, "Clusters");
        if (!Directory.Exists(clp))
            Directory.CreateDirectory(clp);

        Type cl = typeof(ICluster);
        foreach (string d in Directory.EnumerateDirectories(clp))
        {
            string name = Path.GetFileName(d);
            Log.Information("Loading Cluster {Directory}", name);

            AssemblyLoadContext ctx = new(name, true);
            ctx.Resolving += (context, nm) =>
            {
                string dp = Path.Combine(d, $"{nm.Name}.dll");
                return File.Exists(dp) ? context.LoadFromAssemblyPath(dp) : null;
            };

            _asmContexts.Add(name, ctx);
            
            Assembly c;
            try
            {
                c = ctx.LoadFromAssemblyPath(Path.Combine(d, $"{name}.dll"));
            }
            catch (BadImageFormatException ex)
            {
                ctx.Unload();
                _asmContexts.Remove(name);

                continue;
            }

            var clusters = c.GetTypes()
                            .Where(t => cl.IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

            foreach (Type cluster in clusters)
                try
                {
                    // I think this can throw an exception so we do have to catch
                    if (Activator.CreateInstance(cluster, this) is ICluster cc)
                    {
                        _clusters.Add(cc.GetType(), cc);
                        Log.Information("Registered cluster {Cluster} v{Version}", cc.GetName(), cc.GetVersion());
                    }
                    else
                    {
                        Log.Error("Couldn't create instance of {Type}", nameof(cluster));
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Couldn't create instance of {Type}: {Ex}", nameof(cluster), e);
                }
        }
    }

    private async Task InitBuilder(WebApplicationBuilder builder)
    {
        await SetupApplicationBuilder(builder);
        foreach (var c in _clusters) await c.Value.SetupApplicationBuilder(builder);
    }

    private async Task InitClusters()
    {
        App.Use(Middleware);
        await SetupApplication(App);
        foreach (var c in _clusters)
        {
            await c.Value.SetupApplication(App);
            Log.Information("Set up Cluster {cluster}", c.Value.GetName());
        }
    }

    private static async Task Middleware(HttpContext context, Func<Task> next)
    {
        context.Response.Headers.XPoweredBy = $"OpenStar v{Instance.GetVersion()}";

        string? ip = context.Request.Headers["X-OpenStar-Ip"]
                            .FirstOrDefault(context.Connection.RemoteIpAddress?.ToString());

        Log.Information("[{Method} | {Ip}] {Path}", context.Request.Method, ip,
                        context.Request.GetEncodedPathAndQuery());
        await next.Invoke();
    }
}