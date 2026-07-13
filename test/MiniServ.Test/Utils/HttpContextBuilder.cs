using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MiniServ.Test.Utils;

internal sealed class HttpContextBuilder
{
    private readonly HttpContext httpContext = new DefaultHttpContext();
    private readonly IServiceCollection services = new ServiceCollection();

    private HttpContextBuilder()
    {
        httpContext.Request.Method = "GET";
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 80);

        httpContext.Response.Body = new MemoryStream();

        WithService<ILoggerFactory, NullLoggerFactory>();
    }

    public HttpContextBuilder WithMethod(string method)
    {
        httpContext.Request.Method = method;
        return this;
    }

    public HttpContextBuilder WithRouteValue(string key, object? value)
    {
        httpContext.Request.RouteValues[key] = value;
        return this;
    }

    public HttpContextBuilder WithHeader(Action<RequestHeaders> configure)
    {
        configure(httpContext.Request.GetTypedHeaders());
        return this;
    }

    public HttpContextBuilder WithService<TService>()
        where TService : class
    {
        services.AddSingleton<TService, TService>();
        return this;
    }

    public HttpContextBuilder WithService<TInterface, TService>()
        where TInterface : class
        where TService : class, TInterface
    {
        services.AddSingleton<TInterface, TService>();
        return this;
    }

    public HttpContextBuilder WithService<TService>(TService instance)
        where TService : class
    {
        services.AddSingleton(instance);
        return this;
    }

    public HttpContext Build()
    {
        httpContext.RequestServices = services.BuildServiceProvider();

        return httpContext;
    }

    public static HttpContextBuilder Create()
    {
        return new HttpContextBuilder();
    }
}
