using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenStar.Core.Extensions;
using ILogger = Serilog.ILogger;

namespace OpenStar.Core.Endpoint;

/// <summary>
///     General API interface, to be inherited by all API classes
///     Holds a group of Endpoints
/// </summary>
public interface IApi
{
    /// <summary>
    ///     API Group
    ///     e.g /mygroup
    /// </summary>
    string EndpointGroup { get; }

    /// <summary>
    ///     List of API Endpoints
    /// </summary>
    List<Endpoint> Endpoints { get; }

    /// <summary>
    ///     Logger
    /// </summary>
    ILogger Logger { get; }


    /// <summary>
    ///     Registers all endpoints with a WebApplication, and provides API docs
    /// </summary>
    /// <param name="builder">The builder to register the endpoints for</param>
    public ValueTask SetupEndpoints(IEndpointRouteBuilder builder)
    {
        Endpoints.ForEach(e =>
        {
            if (e.DeveloperOnly && OpenStarCore.Instance != null && OpenStarCore.Instance.Client.IsDevelopmentEnvironment())
                return;

            RouteHandlerBuilder b = builder.Map($"{EndpointGroup}/{e.Path}", e.Method, e.Handler)
                                           .WithName(e.Name)
                                           .WithDisplayName(e.DisplayName)
                                           .WithDescription(e.Description)
                                           .WithTags(e.Group);

            b = e.Authorized ? b.RequireAuthorization() : b.AllowAnonymous();

            if (e.Authorized)
                b.ProducesProblem(401);

            foreach (Endpoint.ResponseType r in e.Responses)
                if (r.StatusCode.IsSuccessStatusCode())
                    b.Produces(r.StatusCode, r.Type, r.ContentType, r.AdditionalContentTypes);
                else
                    b.ProducesProblem(r.StatusCode, r.ContentType);

            foreach (IEndpointFilter f in e.Filters)
                b.AddEndpointFilter(f);

            Logger.Information("Registered endpoint {name} ({path})", e.DisplayName, e.Path);
        });

        return ValueTask.CompletedTask;
    }
}