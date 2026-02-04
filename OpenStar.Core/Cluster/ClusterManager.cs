namespace OpenStar.Core.Cluster;

/// <summary>
/// Holds and manages Clusters
/// </summary>
public class ClusterManager
{
    /// <summary>
    /// Registered Clusters
    /// </summary>
    public readonly Dictionary<Type, ICluster> Clusters = [];

    /// <summary>
    /// Gets a Cluster instance by type
    /// </summary>
    /// <typeparam name="T">The type of the Cluster</typeparam>
    /// <returns>The Cluster instance, or null</returns>
    public T? TryGetCluster<T>() where T : Cluster
        => Clusters[typeof(T)] as T;

    /// <summary>
    /// Gets a Cluster instance by type
    /// </summary>
    /// <typeparam name="T">The type of the Cluster, or null if not found</typeparam>
    /// <returns>Whether the Cluster was found</returns>
    public bool TryGetCluster<T>(out T? cluster) where T : Cluster
    {
        cluster = Clusters[typeof(T)] as T;

        return cluster != null;
    }

    internal void Add<T>(T cluster) where T : ICluster
    {
        Clusters[typeof(T)] = cluster;
    }
}