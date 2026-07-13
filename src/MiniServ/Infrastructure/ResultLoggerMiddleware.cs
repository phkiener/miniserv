namespace MiniServ.Infrastructure;

public sealed class ResultLoggerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        var status = context.Response.StatusCode switch
        {
            StatusCodes.Status200OK => "200 OK",
            StatusCodes.Status304NotModified => "304 Not Modified",
            StatusCodes.Status400BadRequest => "400 Bad Request",
            StatusCodes.Status404NotFound => "404 Not Found",
            StatusCodes.Status405MethodNotAllowed => "405 Not Found",
            StatusCodes.Status412PreconditionFailed => "412 Precondition Failed",
            _ => null
        };

        if (status is not null)
        {
            Console.WriteLine($"{status} => {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
        }
    }
}
