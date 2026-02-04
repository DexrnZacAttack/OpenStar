using Microsoft.AspNetCore.Http.Extensions;

namespace OpenStar.Endpoint;

/// <summary>
/// OpenStar Middleware
/// </summary>
public static class Middleware
{
    internal static async Task Invoke(HttpContext context, Func<Task> next)
    {
        context.Response.Headers.XPoweredBy = $"OpenStar v{OpenStar.Instance.GetVersion()}";

        string? ip = context.Request.Headers["X-OpenStar-Ip"]
                            .FirstOrDefault(context.Connection.RemoteIpAddress?.ToString());

        OpenStar.Instance.Logger.Information("[{Method} | {Ip}] {Path}", context.Request.Method, ip,
                                      context.Request.GetEncodedPathAndQuery());
        await next.Invoke();
    }
}