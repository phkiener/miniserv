namespace MiniServ;

public sealed class FileServerOptions
{
    public string ContentRoot { get; private set; } = Environment.CurrentDirectory;

    public bool Version { get; private set; } = false;
    public bool Help { get; private set; } = false;

    public static FileServerOptions? Parse(string[] args)
    {
        var instance = new FileServerOptions();
        if (args.Contains("-h") || args.Contains("--help"))
        {
            instance.Help = true;
        }

        if (args.Contains("-v") || args.Contains("--version"))
        {
            instance.Version = true;
        }

        var lastArgument = args.LastOrDefault();
        if (lastArgument is not null && !lastArgument.StartsWith("-"))
        {
            instance.ContentRoot = lastArgument;
        }

        return instance;
    }

    public static void PrintUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: miniserv [options] [PATH]");
        writer.WriteLine();
        writer.WriteLine("Serves all files under PATH, defaulting to the current directory.");
        writer.WriteLine("Options:");
        writer.WriteLine("  -v|--version  Print version information and quit");
        writer.WriteLine("  -h|--help     Print this help text and quit");
    }
}
