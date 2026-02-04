using Microsoft.AspNetCore.Builder;
using OpenStar.Core;

// ReSharper disable CheckNamespace
namespace OpenStar.Events;

public static class OpenStarEvents
{
    /// <summary>
    /// Invoked before the client's WebApplication is started to allow for modification
    /// </summary>
    public static event Func<WebApplication, Task>? InitializeWebApplication;
    /// <summary>
    /// Invoked before the client's WebApplicationBuilder is built to allow for modification
    /// </summary>
    public static event Func<WebApplicationBuilder, Task>? InitializeWebApplicationBuilder;
    /// <summary>
    /// Invoked to initialize all clusters
    /// </summary>
    public static event Func<IOpenStarClient, Task>? InitializeClusters;

    /// <summary>
    /// Invokes the InitializeWebApplication event, which allows subscribers to modify the application before it's started
    /// </summary>
    /// <param name="application">the WebApplication</param>
    internal static async Task OnWebApplicationInitialize(WebApplication application)
    {
        await (InitializeWebApplication?.Invoke(application) ?? Task.CompletedTask);
    }

    /// <summary>
    /// Invokes the InitializeWebApplicationBuilder event, which allows subscribers to modify the application builder before it's built
    /// </summary>
    /// <param name="builder">the WebApplicationBuilder</param>
    internal static async Task OnWebApplicationBuilderInitialize(WebApplicationBuilder builder)
    {
        await (InitializeWebApplicationBuilder?.Invoke(builder) ?? Task.CompletedTask);
    }

    /// <summary>
    /// Invokes the InitializeClusters event, which allows subscribers to initialize themselves
    /// </summary>
    /// <param name="client">the Client</param>
    internal static async Task OnClusterInitialize(IOpenStarClient client)
    {
        await (InitializeClusters?.Invoke(client) ?? Task.CompletedTask);
    }
}