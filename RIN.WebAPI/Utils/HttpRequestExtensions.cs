using Microsoft.AspNetCore.Http;

namespace RIN.WebAPI.Utils
{
    public static class HttpRequestExtensions
    {
        public static string? GetSingleHeader(this HttpRequest request, string headerName)
        {
            return request.Headers.TryGetValue(headerName, out var header) ? header.FirstOrDefault() : null;
        }
    }
}
