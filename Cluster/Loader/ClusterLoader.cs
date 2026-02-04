using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar.Cluster.Loader;

/// <summary>
/// Loads and registers Clusters
/// </summary>
public abstract class ClusterLoader
{
    /// <summary>
    /// Manager instance where we register our Clusters
    /// </summary>
    protected readonly ClusterManager Manager;

    /// <summary>
    /// Logger used by the ClusterLoader
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Creates a new ClusterLoader
    /// </summary>
    /// <param name="mgr">Manager to register the Clusters in</param>
    /// <param name="logger">Logger to use</param>
    protected ClusterLoader(ClusterManager mgr, ILogger logger)
    {
        this.Logger = logger;
        this.Manager = mgr;
    }

    /// <summary>
    /// Loads all Clusters
    /// </summary>
    public abstract void Load();
    /// <summary>
    /// Registers all Clusters
    /// </summary>
    public abstract void Register();
}