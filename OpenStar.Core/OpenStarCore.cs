namespace OpenStar.Core;

/// <summary>
/// Core class, created by the client
/// </summary>
public class OpenStarCore
{
    /// <summary>
    /// The instance, created by the client at runtime
    /// </summary>
    public static OpenStarCore? Instance { get; internal set; }

    /// <summary>
    /// The Client instance
    /// </summary>
    public IOpenStarClient Client { get; }

    /// <summary>
    /// Creates a new OpenStarCore instance
    /// </summary>
    /// <param name="client">The client</param>
    public OpenStarCore(IOpenStarClient client)
    {
        Client = client;
    }
}