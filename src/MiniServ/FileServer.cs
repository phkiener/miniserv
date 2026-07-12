using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MiniServ.Endpoints;
using MiniServ.Infrastructure;

namespace MiniServ;

public sealed class FileServer(string contentRoot)
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        var options = new WebApplicationOptions { ApplicationName = "MiniServ" };
        var builder = WebApplication.CreateSlimBuilder(options);
        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.Logging.ClearProviders();

        builder.Services.AddScoped<ServeFile>();
        builder.Services.AddScoped<ServeDirectory>();

        builder.Services.AddHostedService<LifetimeLogger>();
        builder.Services.AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>();

        var fileProvider = new PhysicalFileProvider(contentRoot);
        builder.Services.AddSingleton<IFileProvider>(fileProvider);

        var app = builder.Build();

        app.MapGet("{**file}", ServeFile.InvokeAsync);
        app.MapGet("{**directory:nonfile}", ServeDirectory.InvokeAsync);

        cancellationToken.Register(() => app.StopAsync(cancellationToken));
        return app.RunAsync("https://localhost:5000");
    }
}
