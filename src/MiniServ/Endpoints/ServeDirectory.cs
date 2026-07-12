namespace MiniServ.Endpoints;

public sealed class ServeDirectory : IEndpoint<ServeDirectory>
{
    public Task ExecuteAsync(HttpContext context)
    {
        var directory = context.Request.RouteValues["directory"] as string ?? "/";
        var file = Path.Combine(directory, "index.html");
        context.Request.RouteValues["file"] = file;

        return ServeFile.InvokeAsync(context);
    }

    public static Task InvokeAsync(HttpContext context) => context.InvokeAsync<ServeDirectory>();
}
