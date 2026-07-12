using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace MiniServ.Endpoints;

public sealed class ServeFile(IContentTypeProvider contentTypeProvider, IFileProvider fileProvider) : IEndpoint<ServeFile>
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        var filePath = httpContext.Request.RouteValues["file"] as string;
        if (filePath is null)
        {
            return httpContext.ExecuteAsync(Results.BadRequest());
        }

        var fileInfo = fileProvider.GetFileInfo(filePath);
        if (!fileInfo.Exists)
        {
            return httpContext.ExecuteAsync(Results.NotFound());
        }

        if (contentTypeProvider.TryGetContentType(filePath, out var contentType))
        {
            httpContext.Response.ContentType = contentType;
        }

        return httpContext.Response.SendFileAsync(fileInfo, httpContext.RequestAborted);
    }

    public static Task InvokeAsync(HttpContext context) => context.InvokeAsync<ServeFile>();
}
