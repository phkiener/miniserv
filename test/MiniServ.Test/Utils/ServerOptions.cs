namespace MiniServ.Test.Utils;

public sealed class ServerOptions : IServerOptions
{
    public bool HandleNotFound { get; set; } = false;
    public bool PreventCaching { get; set; } = false;
}
