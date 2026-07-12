namespace MiniServ.Endpoints;

public interface IEndpoint<T> where T : IEndpoint<T>
{
    public Task ExecuteAsync(HttpContext context);

    public static abstract Task InvokeAsync(HttpContext context);
}

public static class EndpointExtensions
{
    public static Task InvokeAsync<T>(this HttpContext context) where T : IEndpoint<T>
    {
        var handler = context.RequestServices.GetRequiredService<T>();
        return handler.ExecuteAsync(context);
    }

    public static Task ExecuteAsync(this HttpContext context, IResult result)
    {
        return result.ExecuteAsync(context);
    }
}
