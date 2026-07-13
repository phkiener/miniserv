namespace MiniServ.Infrastructure;

public sealed class ResultLoggerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        var (status, color) = context.Response.StatusCode switch
        {
            StatusCodes.Status200OK => ("200 OK", "white"),
            StatusCodes.Status304NotModified => ("304 Not Modified", "darkblue"),
            StatusCodes.Status400BadRequest => ("400 Bad Request", "darkred"),
            StatusCodes.Status404NotFound => ("404 Not Found", "darkred"),
            StatusCodes.Status405MethodNotAllowed => ("405 Not Found", "darkred"),
            StatusCodes.Status412PreconditionFailed => ("412 Precondition Failed", "darkred"),
            _ => (null, null)
        };

        if (status is not null)
        {
            Console.WriteMarkupLine($" [${color}]{status}[$] => {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
        }
    }
}
