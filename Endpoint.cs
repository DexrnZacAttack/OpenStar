namespace OpenStar;

/// <summary>
///     API endpoint info
/// </summary>
/// <param name="path">Endpoint path</param>
/// <param name="method">HTTP method</param>
/// <param name="handler">Handler function</param>
public class Endpoint(string path, HttpMethod method, Delegate handler, bool authorized = false)
{
    /// <summary>
    ///     Handler function
    /// </summary>
    public readonly Delegate Handler = handler;

    /// <summary>
    ///     HTTP method type
    /// </summary>
    public readonly HttpMethod Method = method;

    /// <summary>
    ///     Path to API endpoint
    /// </summary>
    public readonly string Path = path;

    /// <summary>
    ///     Whether the endpoint requires authorization
    /// </summary>
    public bool Authorized = authorized;

    /// <summary>
    ///     Description of the endpoint
    /// </summary>
    public string Description = path;

    /// <summary>
    ///     Whether the API is only accessible when building with Debug
    /// </summary>
    public bool DeveloperOnly = false;

    /// <summary>
    ///     Display name of the endpoint
    /// </summary>
    public string DisplayName = path;

    /// <summary>
    ///     Methods to run before calling the handler
    /// </summary>
    public IEndpointFilter[] Filters = [];

    /// <summary>
    ///     Endpoint group
    /// </summary>
    public string Group = "";

    /// <summary>
    ///     Name of the endpoint
    /// </summary>
    public string Name = path;

    /// <summary>
    ///     Possible responses
    /// </summary>
    public ResponseType[] Responses = [];

    /// <summary>
    ///     API response type
    /// </summary>
    /// <param name="statusCode">HTTP response code</param>
    /// <param name="type">Response type</param>
    /// <param name="contentType">Response content type</param>
    public class ResponseType(int statusCode, Type? type = null, string? contentType = null)
    {
        /// <summary>
        ///     Guaranteed content type (null if none)
        /// </summary>
        public readonly string? ContentType = contentType;

        /// <summary>
        ///     Response code
        /// </summary>
        public readonly int StatusCode = statusCode;

        /// <summary>
        ///     Response type
        /// </summary>
        public readonly Type? Type = type;

        /// <summary>
        ///     Extra content types
        /// </summary>
        public string[] AdditionalContentTypes = [];
    }
}