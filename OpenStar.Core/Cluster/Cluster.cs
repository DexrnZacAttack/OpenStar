using Microsoft.AspNetCore.Builder;
using OpenStar.Core.Cluster.Config;
using OpenStar.Core.Cluster.Util;
using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar.Core.Cluster;

/// <summary>
/// An OpenStar module/plugin
/// </summary>
public abstract class Cluster : ICluster
{
    /// <summary>
    /// The OpenStar instance which owns the Cluster
    /// </summary>
    public readonly IOpenStarClient Owner;
    
    /// <inheritdoc />
    public abstract ILogger Logger { get; }
    
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Version { get; }

    /// <inheritdoc />
    public string StorageDirectory => Path.Join(Owner.StorageDirectory, Name);

    /// <inheritdoc />
    public abstract IClusterConfig? Config { get; }

    /// <summary>
    /// Creates a new Cluster
    /// </summary>
    /// <param name="owner">The OpenStar instance which owns the Cluster</param>
    protected Cluster(IOpenStarClient owner)
    {
        Owner = owner;

        string d = StorageDirectory;
        if (!Path.Exists(d))
            Directory.CreateDirectory(d);
    }
}