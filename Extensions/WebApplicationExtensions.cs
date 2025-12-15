namespace OpenStar.Extensions;

public static class WebApplicationExtensions
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints, string path, HttpMethod method,
                                          Delegate handler)
    {
        return method.Method switch
        {
            "GET"    => endpoints.MapGet(path, handler),
            "PUT"    => endpoints.MapPut(path, handler),
            "POST"   => endpoints.MapPost(path, handler),
            "DELETE" => endpoints.MapDelete(path, handler),
            "PATCH"  => endpoints.MapPatch(path, handler),
            _        => endpoints.Map(path, handler)
        };
    }
}