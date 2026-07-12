using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MiniServ.Endpoints;
using MiniServ.Infrastructure;

namespace MiniServ;

public sealed class FileServer(FileServerOptions options)
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { ApplicationName = "MiniServ" });
        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.Logging.ClearProviders();

        builder.Services.AddScoped<ServeFile>();
        builder.Services.AddScoped<ServeDirectory>();

        builder.Services.AddHostedService<LifetimeLogger>();
        builder.Services.AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>();

        var fileProvider = Path.IsPathRooted(options.ContentRoot)
            ? new PhysicalFileProvider(options.ContentRoot)
            : new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, options.ContentRoot));

        builder.Services.AddSingleton<IFileProvider>(fileProvider);

        var app = builder.Build();

        app.MapGet("{**file}", ServeFile.InvokeAsync);
        app.MapGet("{**directory:nonfile}", ServeDirectory.InvokeAsync);

        cancellationToken.Register(() => app.StopAsync(cancellationToken));

        return cancellationToken.IsCancellationRequested
            ? Task.CompletedTask
            : app.RunAsync("https://localhost:5000");
    }
}
