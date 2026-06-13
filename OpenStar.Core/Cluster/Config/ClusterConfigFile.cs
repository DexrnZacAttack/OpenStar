using System.Text.Json;

namespace OpenStar.Core.Cluster.Config;

/// <summary>
/// Handles (de)serializing Cluster configs
/// </summary>
public static class ClusterConfigFile
{
    /// <summary>
    /// Default JSON serialization options
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Loads the config from a given root
    /// </summary>
    /// <param name="root">The root directory to look for config.json</param>
    /// <typeparam name="T">The config type to load</typeparam>
    /// <returns>The loaded config</returns>
    /// <exception cref="JsonException">If the config file couldn't be deserialized</exception>
    public static async Task<T> LoadAsync<T>(string root) where T : IClusterConfig, new()
    {
        string p = Path.Combine(root, "config.json");

        if (!File.Exists(p))
        {
            // new
            T c = new();

            // create file
            await using FileStream ofs = File.Create(p);
            await JsonSerializer.SerializeAsync(ofs, c, Options);

            // return our new settings
            return c;
        }

        // load from json
        await using FileStream ifs = File.OpenRead(p);
        return await JsonSerializer.DeserializeAsync<T>(ifs, Options) ??
               throw new JsonException("Couldn't load config");
    }

    /// <summary>
    /// Loads the config for a given Cluster
    /// </summary>
    /// <param name="cluster">The Cluster to load the config for, we search it's StorageDirectory for config.json.</param>
    /// <typeparam name="T">The config type to load</typeparam>
    /// <returns>The loaded config</returns>
    /// <exception cref="JsonException">If the config file couldn't be deserialized</exception>
    public static async Task<T> LoadAsync<T>(ICluster cluster) where T : IClusterConfig, new()
        => await LoadAsync<T>(cluster.StorageDirectory);

    /// <summary>
    /// Writes a given config into config.json inside the given root directory
    /// </summary>
    /// <param name="config">The config we want to serialize</param>
    /// <param name="root">The folder where the resulting config.json will be written</param>
    public static async Task WriteAsync(IClusterConfig config, string root)
    {
        string p = Path.Combine(root, "config.json");

        await using FileStream ofs = new(p, FileMode.OpenOrCreate, FileAccess.Write);
        await JsonSerializer.SerializeAsync(ofs, config, Options);
    }

    /// <summary>
    /// Writes the cluster's config into it's config.json
    /// </summary>
    /// <param name="cluster">The cluster which holds the config we want to serialize</param>
    public static async Task WriteAsync(ICluster cluster)
    {
        IClusterConfig? c = cluster.Config;
        if (c != null)
            await WriteAsync(c, cluster.StorageDirectory);
    }

    /// <summary>
    /// Loads the config from a given root
    /// </summary>
    /// <param name="root">The root directory to look for config.json</param>
    /// <typeparam name="T">The config type to load</typeparam>
    /// <returns>The loaded config</returns>
    /// <exception cref="JsonException">If the config file couldn't be deserialized</exception>
    public static T Load<T>(string root) where T : IClusterConfig, new()
    {
        string p = Path.Combine(root, "config.json");

        if (!File.Exists(p))
        {
            // new
            T c = new();

            // create file
            using FileStream ofs = File.Create(p);
            JsonSerializer.Serialize(ofs, c, Options);

            // return our new settings
            return c;
        }

        // load from json
        using FileStream ifs = File.OpenRead(p);
        return JsonSerializer.Deserialize<T>(ifs, Options) ?? throw new JsonException("Couldn't load config");
    }

    /// <summary>
    /// Loads the config for a given Cluster
    /// </summary>
    /// <param name="cluster">The Cluster to load the config for, we search it's StorageDirectory for config.json.</param>
    /// <typeparam name="T">The config type to load</typeparam>
    /// <returns>The loaded config</returns>
    /// <exception cref="JsonException">If the config file couldn't be deserialized</exception>
    public static T Load<T>(ICluster cluster) where T : IClusterConfig, new()
        => Load<T>(cluster.StorageDirectory);

    /// <summary>
    /// Writes a given config into config.json inside the given root directory
    /// </summary>
    /// <param name="config">The config we want to serialize</param>
    /// <param name="root">The folder where the resulting config.json will be written</param>
    public static void Write(IClusterConfig config, string root)
    {
        string p = Path.Combine(root, "config.json");

        using FileStream ofs = new(p, FileMode.OpenOrCreate, FileAccess.Write);
        JsonSerializer.Serialize(ofs, config, Options);
    }

    /// <summary>
    /// Writes the cluster's config into it's config.json
    /// </summary>
    /// <param name="cluster">The cluster which holds the config we want to serialize</param>
    public static void Write(ICluster cluster)
    {
        IClusterConfig? c = cluster.Config;
        if (c != null)
            Write(c, cluster.StorageDirectory);
    }

    /// <summary>
    /// Loads the config for a given Cluster
    /// </summary>
    /// <param name="cluster">The Cluster to load the config for, we search it's StorageDirectory for config.json.</param>
    /// <typeparam name="T">The config type to load</typeparam>
    /// <returns>The loaded config</returns>
    /// <exception cref="JsonException">If the config file couldn't be deserialized</exception>
    public static async Task<T> LoadConfigAsync<T>(this ICluster cluster) where T : IClusterConfig, new()
        => await LoadAsync<T>(cluster);

    /// <summary>
    /// Writes the cluster's config into it's config.json
    /// </summary>
    /// <param name="cluster">The cluster which holds the config we want to serialize</param>
    public static async Task WriteConfigAsync(this ICluster cluster)
        => await WriteAsync(cluster);

    /// <summary>
    /// Loads the config for a given Cluster
    /// </summary>
    /// <param name="cluster">The Cluster to load the config for, we search it's StorageDirectory for config.json.</param>
    /// <typeparam name="T">The config type to load</typeparam>
    /// <returns>The loaded config</returns>
    /// <exception cref="JsonException">If the config file couldn't be deserialized</exception>
    public static T LoadConfig<T>(this ICluster cluster) where T : IClusterConfig, new()
        => Load<T>(cluster);

    /// <summary>
    /// Writes the cluster's config into it's config.json
    /// </summary>
    /// <param name="cluster">The cluster which holds the config we want to serialize</param>
    public static void WriteConfig(this ICluster cluster)
        => Write(cluster);
}