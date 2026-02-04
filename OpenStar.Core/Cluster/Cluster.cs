using Microsoft.AspNetCore.Builder;
using OpenStar.Core.Cluster.Config;
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
    protected readonly IOpenStarClient Owner;

    /// <summary>
    /// Creates a new Cluster
    /// </summary>
    /// <param name="owner">The OpenStar instance which owns the Cluster</param>
    protected Cluster(IOpenStarClient owner)
    {
        Owner = owner;
        CreateLogger();

        string d = GetStorageDirectory();
        if (!Path.Exists(d))
            Directory.CreateDirectory(d);
    }

    /// <inheritdoc />
    public ILogger CreateLogger() =>
        Owner.Logger.ForContext("SourceContext", GetName());

    /// <inheritdoc />
    public virtual string GetStorageDirectory() => Path.Join(Owner.GetStorageDirectory(), GetName());

    /// <inheritdoc />
    public abstract ILogger Logger { get; init; }

    /// <inheritdoc />
    public abstract string GetName();
    /// <inheritdoc />
    public abstract string GetVersion();

    /// <inheritdoc />
    public abstract Task SetupApplicationBuilder(WebApplicationBuilder builder);
    /// <inheritdoc />
    public abstract Task SetupApplication(WebApplication app);

    /// <inheritdoc />
    public abstract ClusterConfig? GetConfig();
}