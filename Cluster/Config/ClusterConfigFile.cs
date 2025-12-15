using System.Text.Json;

namespace OpenStar.Cluster.Config;

public static class ClusterConfigFile
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static async Task<T> LoadAsync<T>(string root) where T : ClusterConfig, new()
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
               throw new NullReferenceException("Couldn't load config");
    }

    public static async Task<T> LoadAsync<T>(ICluster cluster) where T : ClusterConfig, new()
        => await LoadAsync<T>(cluster.GetStorageDirectory());

    private static async Task WriteAsync(ClusterConfig config, string root)
    {
        string p = Path.Combine(root, "config.json");

        await using FileStream ofs = new(p, FileMode.OpenOrCreate, FileAccess.Write);
        await JsonSerializer.SerializeAsync(ofs, config, Options);
    }

    public static async Task WriteAsync(ICluster cluster)
    {
        ClusterConfig? c = cluster.GetConfig();
        if (c != null)
            await WriteAsync(c, cluster.GetStorageDirectory());
    }

    public static async Task<T> LoadConfigAsync<T>(this ICluster cluster) where T : ClusterConfig, new()
        => await LoadAsync<T>(cluster);

    public static async Task WriteConfigAsync(this ICluster cluster)
        => await WriteAsync(cluster);

    public static T Load<T>(string root) where T : ClusterConfig, new()
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
        return JsonSerializer.Deserialize<T>(ifs, Options) ?? throw new NullReferenceException("Couldn't load config");
    }

    public static T Load<T>(ICluster cluster) where T : ClusterConfig, new()
        => Load<T>(cluster.GetStorageDirectory());

    private static void Write(ClusterConfig config, string root)
    {
        string p = Path.Combine(root, "config.json");

        using FileStream ofs = new(p, FileMode.OpenOrCreate, FileAccess.Write);
        JsonSerializer.Serialize(ofs, config, Options);
    }

    public static void Write(ICluster cluster)
    {
        ClusterConfig? c = cluster.GetConfig();
        if (c != null)
            Write(c, cluster.GetStorageDirectory());
    }

    public static T LoadConfig<T>(this ICluster cluster) where T : ClusterConfig, new()
        => Load<T>(cluster);

    public static void WriteConfig(this ICluster cluster)
        => Write(cluster);
}