using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace MiniServ.Endpoints;

public sealed class ServeFile(IContentTypeProvider contentTypeProvider, IFileProvider fileProvider) : IEndpoint<ServeFile>
{
    public Task ExecuteAsync(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Get && context.Request.Method != HttpMethods.Head)
        {
            return context.ExecuteAsync(Results.StatusCode(StatusCodes.Status405MethodNotAllowed));
        }

        if (context.Request.RouteValues["file"] is not string filePath)
        {
            return context.ExecuteAsync(Results.BadRequest());
        }

        var fileInfo = fileProvider.GetFileInfo(filePath);
        if (!fileInfo.Exists)
        {
            return context.ExecuteAsync(Results.NotFound());
        }

        var requestHeaders = context.Request.GetTypedHeaders();
        if (requestHeaders.IfModifiedSince.HasValue && requestHeaders.IfModifiedSince >= fileInfo.LastModified)
        {
            return context.ExecuteAsync(Results.StatusCode(StatusCodes.Status304NotModified));
        }

        if (requestHeaders.IfUnmodifiedSince.HasValue && requestHeaders.IfUnmodifiedSince < fileInfo.LastModified)
        {
            return context.ExecuteAsync(Results.StatusCode(StatusCodes.Status412PreconditionFailed));
        }

        SetResponseHeaders(fileInfo, context.Response.GetTypedHeaders());

        return context.Request.Method == HttpMethods.Get
            ? context.Response.SendFileAsync(fileInfo, context.RequestAborted)
            : Task.CompletedTask;
    }

    private void SetResponseHeaders(IFileInfo file, ResponseHeaders headers)
    {
        headers.ContentLength = file.Length;
        headers.LastModified = file.LastModified;

        if (contentTypeProvider.TryGetContentType(file.Name, out var contentType))
        {
            headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
    }

    public static Task InvokeAsync(HttpContext context) => context.InvokeAsync<ServeFile>();
}
