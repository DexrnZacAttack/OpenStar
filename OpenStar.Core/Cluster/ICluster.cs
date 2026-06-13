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
    public string Name { get; }

    /// <summary>
    ///     Gets the version of the Cluster
    /// </summary>
    /// <returns>The Cluster's version</returns>
    public string Version { get; }

    /// <summary>
    ///     Gets the storage directory of the Cluster
    /// </summary>
    /// <returns>The Cluster's storage directory</returns>
    public string StorageDirectory { get; }

    /// <summary>
    /// Gets config settings for a cluster
    /// </summary>
    /// <returns>Config settings, if present</returns>
    public IClusterConfig? Config { get; }
}