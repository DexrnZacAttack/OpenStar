namespace OpenStar.Extensions;

/// <summary>
///     Extensions for various integer types
/// </summary>
public static class IntegerExtensions
{
    /// <summary>
    ///     Checks if the given integer is an HTTP status code indicating success
    /// </summary>
    /// <param name="statusCode">The code</param>
    /// <returns>True if success, otherwise False</returns>
    public static bool IsSuccessStatusCode(this int statusCode) => statusCode is >= 200 and < 300;
}