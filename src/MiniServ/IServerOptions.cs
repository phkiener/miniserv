namespace MiniServ;

public interface IServerOptions
{
    public bool HandleNotFound { get; }
    public bool PreventCaching { get; }
}
