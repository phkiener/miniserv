using Microsoft.AspNetCore.Http;

namespace MiniServ.Test;

internal static class HttpContextExtensions
{
    extension(HttpResponse response)
    {
        public byte[] Content => CopyResponseBody(response);
    }

    private static byte[] CopyResponseBody(HttpResponse response)
    {
        if (response.Body is MemoryStream ms)
        {
            return ms.ToArray();
        }

        throw new NotSupportedException($"Body is not a {nameof(MemoryStream)}, this shouldn't happen in a test.");
    }
}
