using Microsoft.AspNetCore.Builder;
using OpenStar.Core.Cluster.Config;
using ILogger = Serilog.ILogger;

namespace OpenStar.Core.Cluster;

/// <summary>
///     OpenStar extension interface
/// </summary>
public interface ICluster
{
    /// <summary>
    /// Cluster logger
    /// </summary>
    public ILogger Logger { get; }

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
    /// Gets config settings for a cluster
    /// </summary>
    /// <returns>Config settings, if present</returns>
    public ClusterConfig? GetConfig();

    /// <summary>
    ///     Creates a logger for the Cluster
    /// </summary>
    /// <returns>The new logger</returns>
    protected ILogger CreateLogger();

    /// <summary>
    /// Called during initialization of the WebApplication to let each Cluster modify it
    /// </summary>
    /// <param name="app">OpenStar's WebApplication</param>
    public Task SetupApplication(WebApplication app);

    /// <summary>
    /// Called during initialization of the WebApplicationBuilder to let each Cluster modify it before building
    /// </summary>
    /// <param name="builder">OpenStar's WebApplicationBuilder</param>
    public Task SetupApplicationBuilder(WebApplicationBuilder builder);
}