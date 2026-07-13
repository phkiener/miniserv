using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MiniServ.Endpoints;

namespace MiniServ.Test.Endpoints;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public sealed class ServeDirectoryTest
{
    private static readonly DateTimeOffset LongAgo = new(2026, 02, 14, 13, 37, 00, TimeSpan.Zero);
    private static readonly byte[] DefaultContent = [0xDE, 0xAD, 0xBE, 0xEF];

    private readonly InMemoryFileProvider fileProvider = new();

    private HttpContextBuilder DefaultBuilder => HttpContextBuilder.Create()
        .WithService<ServeFile>()
        .WithService<ServeDirectory>()
        .WithService<IContentTypeProvider, FileExtensionContentTypeProvider>()
        .WithService<IFileProvider>(fileProvider);

    [Test]
    [TestCase("TRACE")]
    [TestCase("OPTIONS")]
    [TestCase("CONNECT")]
    [TestCase("POST")]
    [TestCase("PUT")]
    [TestCase("PATCH")]
    [TestCase("DELETE")]
    [TestCase("QUERY")]
    public async Task ReturnsMethodNotAllowed_WhenRequestIsNotHeadOrGet(string method)
    {
        var httpContext = DefaultBuilder.WithMethod(method).Build();

        await ServeDirectory.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status405MethodNotAllowed));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsNotFound_WhenFileDoesNotExist_InRoot(string method)
    {
        var httpContext = DefaultBuilder
            .WithMethod(method)
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsNotFound_WhenFileDoesNotExist_InFolder(string method)
    {
        var httpContext = DefaultBuilder
            .WithMethod(method)
            .WithRouteValue("directory", "foo")
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsNotModified_WhenLastModificationIsOlder(string method)
    {
        fileProvider.WithFile(path: "/index.html", lastModified: LongAgo);

        var httpContext = DefaultBuilder
            .WithMethod(method)
            .WithHeader(h => h.IfModifiedSince = DateTimeOffset.UtcNow)
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status304NotModified));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsPreconditionFailed_WhenLastModificationIsOlder(string method)
    {
        fileProvider.WithFile(path: "/index.html", lastModified: DateTimeOffset.UtcNow);

        var httpContext = DefaultBuilder
            .WithMethod(method)
            .WithHeader(h => h.IfUnmodifiedSince = LongAgo)
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status412PreconditionFailed));
    }

    [Test]
    public async Task ReturnsExpectedResponse_OnHeadRequest_ToRoot()
    {
        fileProvider.WithFile(path: "/index.html", lastModified: LongAgo, content: DefaultContent);

        var httpContext = DefaultBuilder
            .WithMethod("HEAD")
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        var typedHeaders = httpContext.Response.GetTypedHeaders();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(httpContext.Response.Body.Length, Is.Zero);
        Assert.That(typedHeaders.ContentType?.MediaType, Is.EqualTo("text/html"));
        Assert.That(typedHeaders.ContentLength, Is.EqualTo(DefaultContent.Length));
        Assert.That(typedHeaders.LastModified, Is.EqualTo(LongAgo));
    }

    [Test]
    public async Task ReturnsExpectedResponse_OnHeadRequest_ToFolder()
    {
        fileProvider.WithFile(path: "/foo/index.html", lastModified: LongAgo, content: DefaultContent);

        var httpContext = DefaultBuilder
            .WithMethod("HEAD")
            .WithRouteValue("directory", "foo")
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        var typedHeaders = httpContext.Response.GetTypedHeaders();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(httpContext.Response.Body.Length, Is.Zero);
        Assert.That(typedHeaders.ContentType?.MediaType, Is.EqualTo("text/html"));
        Assert.That(typedHeaders.ContentLength, Is.EqualTo(DefaultContent.Length));
        Assert.That(typedHeaders.LastModified, Is.EqualTo(LongAgo));
    }

    [Test]
    public async Task ReturnsExpectedResponse_OnGetRequest_ToRoot()
    {
        fileProvider.WithFile(path: "/index.html", lastModified: LongAgo, content: DefaultContent);

        var httpContext = DefaultBuilder
            .WithMethod("GET")
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        var typedHeaders = httpContext.Response.GetTypedHeaders();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(httpContext.Response.Content, Is.EqualTo(DefaultContent));
        Assert.That(typedHeaders.ContentType?.MediaType, Is.EqualTo("text/html"));
        Assert.That(typedHeaders.ContentLength, Is.EqualTo(DefaultContent.Length));
        Assert.That(typedHeaders.LastModified, Is.EqualTo(LongAgo));
    }

    [Test]
    public async Task ReturnsExpectedResponse_OnGetRequest_ToFolder()
    {
        fileProvider.WithFile(path: "/foo/index.html", lastModified: LongAgo, content: DefaultContent);

        var httpContext = DefaultBuilder
            .WithMethod("GET")
            .WithRouteValue("directory", "foo")
            .Build();

        await ServeDirectory.InvokeAsync(httpContext);
        var typedHeaders = httpContext.Response.GetTypedHeaders();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(httpContext.Response.Content, Is.EqualTo(DefaultContent));
        Assert.That(typedHeaders.ContentType?.MediaType, Is.EqualTo("text/html"));
        Assert.That(typedHeaders.ContentLength, Is.EqualTo(DefaultContent.Length));
        Assert.That(typedHeaders.LastModified, Is.EqualTo(LongAgo));
    }
}
