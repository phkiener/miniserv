using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MiniServ.Endpoints;
using MiniServ.Infrastructure;

var options = new WebApplicationOptions { ApplicationName = "MiniServ", Args = args };
var builder = WebApplication.CreateSlimBuilder(options);
builder.WebHost.UseKestrelHttpsConfiguration();
builder.Logging.ClearProviders();

builder.Services.AddScoped<ServeFile>();
builder.Services.AddScoped<ServeDirectory>();

builder.Services.AddHostedService<LifetimeLogger>();
builder.Services.AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>();

var fileProvider = new PhysicalFileProvider(Environment.CurrentDirectory);
builder.Services.AddSingleton<IFileProvider>(fileProvider);

var app = builder.Build();

app.MapGet("{**file}", ServeFile.InvokeAsync);
app.MapGet("{**directory:nonfile}", ServeDirectory.InvokeAsync);

await app.RunAsync("https://localhost:5000");
return 0;
