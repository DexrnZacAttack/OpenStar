using Microsoft.AspNetCore.Builder;
using OpenStar.Core.Cluster;

namespace OpenStar.Core;

/// <summary>
/// Client interface, so that Clusters can still interface with the Client
/// </summary>
///
/// <remarks>
/// Had to do this to get stuff to link properly.
/// </remarks>
public interface IOpenStarClient : ICluster
{
    /// <summary>
    /// The client's Cluster manager, holds Cluster instances
    /// </summary>
    ClusterManager Manager { get; }

    /// <summary>
    /// The ASP.NET WebApplication
    /// </summary>
    WebApplication? App { get; internal set; }

    /// <summary>
    /// Whether OpenStar is running in a development environment
    /// </summary>
    /// <returns>`true` if devenv</returns>
    bool IsDevelopmentEnvironment();
}