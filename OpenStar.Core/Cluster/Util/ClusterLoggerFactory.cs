using Serilog;

namespace OpenStar.Core.Cluster.Util;

/// <summary>
/// Factory for creating simple cluster loggers
/// </summary>
public static class ClusterLoggerFactory
{
    /// <summary>
    /// Creates a logger for a given Cluster
    /// </summary>
    /// <returns>The new logger</returns>
    public static ILogger Create(Cluster cluster) => cluster.Owner.Logger.ForContext("SourceContext", cluster.Name);
}