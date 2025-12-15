using OpenStar.Cluster.Config;
using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar.Cluster;

public abstract class Cluster : ICluster
{
    protected readonly OpenStar Owner;

    protected Cluster(OpenStar owner)
    {
        Owner = owner;
        CreateLogger();

        string d = GetStorageDirectory();
        if (!Path.Exists(d))
            Directory.CreateDirectory(d);
    }

    public ILogger CreateLogger() =>
        Log.ForContext("SourceContext", GetName());

    public virtual string GetStorageDirectory() => Path.Join(Owner.ConfigPath, GetName());

    public abstract string GetName();
    public abstract string GetVersion();
    public abstract Task SetupApplicationBuilder(WebApplicationBuilder builder);
    public abstract Task SetupApplication(WebApplication app);
    public abstract ClusterConfig? GetConfig();
}