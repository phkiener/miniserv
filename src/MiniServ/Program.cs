using System.Reflection;
using MiniServ;
using FileServerOptions = MiniServ.FileServerOptions;

var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationSource.Cancel();

var options = FileServerOptions.Parse(args);
if (options is null || options.Help)
{
    FileServerOptions.PrintUsage(Console.Error);
    return 255;
}

if (options.Version)
{
    var version = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
    Console.Error.WriteLine($"MiniServ version {version?.InformationalVersion ?? "super secret preview"}");
    return 0;
}

var fileServer = new FileServer(options);
await fileServer.RunAsync(cancellationSource.Token);

return 0;
