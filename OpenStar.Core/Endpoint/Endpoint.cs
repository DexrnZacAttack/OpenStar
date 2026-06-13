using Microsoft.AspNetCore.Http;

namespace OpenStar.Core.Endpoint;

/// <summary>
/// API endpoint info
/// </summary>
/// <param name="Path">Path to API endpoint</param>
/// <param name="Method">HTTP method type</param>
/// <param name="Handler">Handler function</param>
public record Endpoint(string Path, HttpMethod Method, Delegate Handler)
{
    /// <summary>
    ///     Name of the endpoint
    /// </summary>
    public string Name { get; init; } = Path;
    
    /// <summary>
    ///     Display name of the endpoint
    /// </summary>
    public string DisplayName { get; init; } =  string.Empty;
    
    /// <summary>
    ///     Description of the endpoint
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    ///     Whether the endpoint requires authorization
    /// </summary>
    public bool Authorized { get; init; } = false;
    
    /// <summary>
    ///     Whether the API is only accessible when building with Debug
    /// </summary>
    public bool DeveloperOnly { get; init; }

    /// <summary>
    ///     Methods to run before calling the handler
    /// </summary>
    public IEndpointFilter[] Filters { get; init; } = [];

    /// <summary>
    ///     Endpoint group
    /// </summary>
    public string Group { get; init; } = string.Empty;

    /// <summary>
    ///     Possible responses
    /// </summary>
    public ResponseType[] Responses { get; init; } = [];

    /// <summary>
    ///     API response type
    /// </summary>
    /// <param name="StatusCode">HTTP response code</param>
    /// <param name="Type">Response type</param>
    /// <param name="ContentType">Guaranteed content type (null if none)</param>
    public record ResponseType(int StatusCode, Type? Type = null, string? ContentType = null)
    {
        /// <summary>
        ///     Extra content types
        /// </summary>
        public string[] AdditionalContentTypes { get; init; } = [];
    }
}