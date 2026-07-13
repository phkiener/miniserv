using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MiniServ.Test.Utils;

internal sealed class InMemoryFileProvider : IFileProvider
{
    private sealed class InMemoryFile(string path, DateTimeOffset lastModified, byte[] content) : IFileInfo
    {
        public Stream CreateReadStream()
        {
            return new MemoryStream(content);
        }

        public string VirtualPath { get; } = path;
        public string Name { get; } = Path.GetFileName(path);
        public DateTimeOffset LastModified { get; } = lastModified;

        public string? PhysicalPath => null;
        public bool Exists => true;
        public bool IsDirectory => false;
        public long Length => content.LongLength;
    }

    private readonly List<InMemoryFile> files = [];

    public InMemoryFileProvider WithFile(string path, DateTimeOffset? lastModified = null, byte[]? content = null)
    {
        var file = new InMemoryFile(path, lastModified ?? DateTimeOffset.UtcNow, content ?? []);
        files.Add(file);

        return this;
    }

    public IDirectoryContents GetDirectoryContents(string subpath) => throw new NotImplementedException();

    public IFileInfo GetFileInfo(string subpath)
    {
        var normalizedPath = subpath.StartsWith('/') ? subpath : '/' + subpath;
        return files.SingleOrDefault(f => f.VirtualPath == normalizedPath) as IFileInfo ?? new NotFoundFileInfo(subpath);
    }

    public IChangeToken Watch(string filter) => throw new NotImplementedException();
}
