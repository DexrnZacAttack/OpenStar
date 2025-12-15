using OpenStar.Cluster.Config;
using ILogger = Serilog.ILogger;

namespace OpenStar.Cluster;

/// <summary>
///     OpenStar extension class
/// </summary>
public interface ICluster
{
    /// <summary>
    ///     Gets the name of the Cluster
    /// </summary>
    /// <returns>The Cluster's name</returns>
    public string GetName();

    /// <summary>
    ///     Gets the version of the Cluster
    /// </summary>
    /// <returns>The Cluster's version</returns>
    public string GetVersion();

    /// <summary>
    ///     Gets the storage directory of the Cluster
    /// </summary>
    /// <returns>The Cluster's storage directory</returns>
    public string GetStorageDirectory();

    /// <summary>
    ///     Gets the Cluster's config
    /// </summary>
    /// <returns>The Cluster's config</returns>
    public ClusterConfig? GetConfig();

    /// <summary>
    ///     Creates a logger for the Cluster
    /// </summary>
    /// <returns>The new logger</returns>
    protected ILogger CreateLogger();

    /// <summary>
    ///     Sets up the Cluster
    /// </summary>
    public Task SetupApplication(WebApplication app);

    public Task SetupApplicationBuilder(WebApplicationBuilder builder);
}