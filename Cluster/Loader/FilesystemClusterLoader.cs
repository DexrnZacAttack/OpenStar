using System.Reflection;
using System.Runtime.Loader;
using Serilog;
using ILogger = Serilog.ILogger;

namespace OpenStar.Cluster.Loader;

/// <summary>
/// Loads Cluster assemblies from the filesystem
/// </summary>
public class FilesystemClusterLoader : ClusterLoader
{
    /// <summary>
    /// Assembly contexts so we can keep track of what Clusters we have loaded
    /// </summary>
    private readonly Dictionary<string, AssemblyLoadContext> _asmContexts = new();

    /// <summary>
    /// Cluster types that we register
    /// </summary>
    private IEnumerable<Type>? _clusterTypes;

    /// <summary>
    /// Where to load Clusters from
    /// </summary>
    private readonly string _storagePath;

    /// <summary>
    /// Creates a new FilesystemClusterLoader
    /// </summary>
    /// <param name="mgr">The ClusterManager to store registered Clusters in</param>
    /// <param name="path">The path to read Cluster assemblies from</param>
    /// <param name="logger">The logger to use</param>
    public FilesystemClusterLoader(ClusterManager mgr, string path, ILogger logger) : base(mgr, logger)
    {
        this._storagePath = path;
    }

    /// <summary>
    /// Creates a new FilesystemClusterLoader
    /// </summary>
    /// <param name="mgr">The ClusterManager to store registered Clusters in</param>
    /// <param name="path">The path to read Cluster assemblies from</param>
    public FilesystemClusterLoader(ClusterManager mgr, string path) : this(mgr, path, Log.ForContext<FilesystemClusterLoader>())
    {
    }

    /// <inheritdoc />
    public override void Load()
    {
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);

        Type cl = typeof(ICluster);
        foreach (string d in Directory.EnumerateDirectories(_storagePath))
        {
            string name = Path.GetFileName(d);
            Logger.Information("Loading Cluster {Directory}", name);

            AssemblyLoadContext ctx = new(name, true);
            ctx.Resolving += (context, nm) =>
            {
                string dp = Path.Combine(d, $"{nm.Name}.dll");
                return File.Exists(dp) ? context.LoadFromAssemblyPath(dp) : null;
            };

            _asmContexts.Add(name, ctx);

            Assembly c;
            try
            {
                c = ctx.LoadFromAssemblyPath(Path.Combine(d, $"{name}.dll"));
            }
            catch (BadImageFormatException ex)
            {
                ctx.Unload();
                _asmContexts.Remove(name);

                continue;
            }

            _clusterTypes = c.GetTypes()
                             .Where(t => cl.IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });
        }
    }

    /// <inheritdoc />
    public override void Register()
    {
        if (_clusterTypes is null)
            return;

        foreach (Type cluster in _clusterTypes)
        {
            try
            {
                // I think this can throw an exception so we do have to catch
                if (Activator.CreateInstance(cluster, this) is ICluster cc)
                {
                    Manager.Add(cc);
                    Logger.Information("Registered cluster {Cluster} v{Version}", cc.GetName(), cc.GetVersion());
                }
                else
                {
                    Logger.Error("Couldn't create instance of {Type}", nameof(cluster));
                }
            }
            catch (Exception e)
            {
                Logger.Error("Couldn't create instance of {Type}: {Ex}", nameof(cluster), e);
            }
        }
    }
}