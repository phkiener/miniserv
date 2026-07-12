using MiniServ;

var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationSource.Cancel();

var fileServer = new FileServer(Environment.CurrentDirectory);
await fileServer.RunAsync(cancellationSource.Token);

return 0;
