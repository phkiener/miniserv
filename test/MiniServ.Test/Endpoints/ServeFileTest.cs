using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MiniServ.Endpoints;

namespace MiniServ.Test.Endpoints;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public sealed class ServeFileTest
{
    private static readonly DateTimeOffset LongAgo = new(2026, 02, 14, 13, 37, 00, TimeSpan.Zero);
    private static readonly byte[] DefaultContent = [0xDE, 0xAD, 0xBE, 0xEF];

    private readonly InMemoryFileProvider fileProvider = new();

    private HttpContextBuilder DefaultBuilder => HttpContextBuilder.Create()
        .WithService<ServeFile>()
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

        await ServeFile.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status405MethodNotAllowed));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsBadRequest_WhenFileIsNotGiven(string method)
    {
        var httpContext = DefaultBuilder.WithMethod(method).Build();

        await ServeFile.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsNotFound_WhenFileDoesNotExist(string method)
    {
        var httpContext = DefaultBuilder
            .WithMethod(method)
            .WithRouteValue("file", "file.txt")
            .Build();

        await ServeFile.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsNotModified_WhenLastModificationIsOlder(string method)
    {
        fileProvider.WithFile(path: "file.txt", lastModified: LongAgo);

        var httpContext = DefaultBuilder
            .WithMethod(method)
            .WithRouteValue("file", "file.txt")
            .WithHeader(h => h.IfModifiedSince = DateTimeOffset.UtcNow)
            .Build();

        await ServeFile.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status304NotModified));
    }

    [Test]
    [TestCase("HEAD")]
    [TestCase("GET")]
    public async Task ReturnsPreconditionFailed_WhenLastModificationIsOlder(string method)
    {
        fileProvider.WithFile(path: "file.txt", lastModified: DateTimeOffset.UtcNow);

        var httpContext = DefaultBuilder
            .WithMethod(method)
            .WithRouteValue("file", "file.txt")
            .WithHeader(h => h.IfUnmodifiedSince = LongAgo)
            .Build();

        await ServeFile.InvokeAsync(httpContext);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status412PreconditionFailed));
    }

    [Test]
    public async Task ReturnsExpectedResponse_OnHeadRequest()
    {
        fileProvider.WithFile(path: "file.txt", lastModified: LongAgo, content: DefaultContent);

        var httpContext = DefaultBuilder
            .WithMethod("HEAD")
            .WithRouteValue("file", "file.txt")
            .Build();

        await ServeFile.InvokeAsync(httpContext);
        var typedHeaders = httpContext.Response.GetTypedHeaders();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(httpContext.Response.Body.Length, Is.Zero);
        Assert.That(typedHeaders.ContentType?.MediaType, Is.EqualTo("text/plain"));
        Assert.That(typedHeaders.ContentLength, Is.EqualTo(DefaultContent.Length));
        Assert.That(typedHeaders.LastModified, Is.EqualTo(LongAgo));
    }

    [Test]
    public async Task ReturnsExpectedResponse_OnGetRequest()
    {
        fileProvider.WithFile(path: "file.txt", lastModified: LongAgo, content: DefaultContent);

        var httpContext = DefaultBuilder
            .WithMethod("GET")
            .WithRouteValue("file", "file.txt")
            .Build();

        await ServeFile.InvokeAsync(httpContext);
        var typedHeaders = httpContext.Response.GetTypedHeaders();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(httpContext.Response.Content, Is.EqualTo(DefaultContent));
        Assert.That(typedHeaders.ContentType?.MediaType, Is.EqualTo("text/plain"));
        Assert.That(typedHeaders.ContentLength, Is.EqualTo(DefaultContent.Length));
        Assert.That(typedHeaders.LastModified, Is.EqualTo(LongAgo));
    }
}
