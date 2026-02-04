using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace OpenStar.Core.Extensions;

/// <summary>
/// Extensions for ASP.NET's WebApplication and related classes
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Generic router map function which calls the intended map function based on the given HttpMethod
    /// </summary>
    /// <param name="route">Route to map</param>
    /// <param name="path">Where to map it to</param>
    /// <param name="method">HTTP method to map</param>
    /// <param name="handler">The function that will handle requests to that route</param>
    /// <returns></returns>
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder route, string path, HttpMethod method,
                                          Delegate handler)
    {
        return method.Method switch
        {
            "GET"    => route.MapGet(path, handler),
            "PUT"    => route.MapPut(path, handler),
            "POST"   => route.MapPost(path, handler),
            "DELETE" => route.MapDelete(path, handler),
            "PATCH"  => route.MapPatch(path, handler),
            _        => route.Map(path, handler)
        };
    }
}