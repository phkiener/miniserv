using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.FileProviders;

namespace MiniServ.Infrastructure;

public sealed class LifetimeLogger(IServer server, IFileProvider fileProvider) : IHostedLifecycleService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        var rootFolder = fileProvider.GetFileInfo("/");

        var serverAddressFeature = server.Features.Get<IServerAddressesFeature>();
        var listenAddress = serverAddressFeature?.Addresses.First();
        if (listenAddress is not null)
        {
            Console.WriteLine($"Serving {rootFolder.PhysicalPath} on {listenAddress}");
        }

        return Task.CompletedTask;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
