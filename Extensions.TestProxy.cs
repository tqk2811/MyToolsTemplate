using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    internal static partial class Extensions
    {
        static readonly IEnumerable<string> _urlChecks = new string[]
        {
            "https://lumtest.com/myip.json",
            "https://checkip.amazonaws.com/",
            "https://domains.google.com/checkip"
        };
        public static async Task<bool> TestProxyLiveAsync(this HttpClient httpClient, CancellationToken cancellationToken = default)
        {
            List<Task<HttpResponseMessage>> tasks = _urlChecks
                    .Select(x => httpClient.GetAsync(x, HttpCompletionOption.ResponseContentRead, cancellationToken))
                    .ToList();
            while (tasks.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await Task.WhenAny(tasks);
                }
                catch (Exception) { }
                if (tasks.Any(x => x.IsCompletedSuccessfully))
                {
                    return true;
                }

                tasks = tasks.Where(x => !x.IsFaulted && !x.IsCanceled).ToList();
            }
            return false;
        }
        public static async Task<bool> TestProxyLiveAsync(this string proxy, ILogger logger, CancellationToken cancellationToken = default)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.UseCookies = true;
            httpClientHandler.CookieContainer = new CookieContainer();
            httpClientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.Brotli | DecompressionMethods.GZip;
            if (!string.IsNullOrWhiteSpace(proxy))
            {
                string[] split = proxy.Split(':');
                if (split.Length == 2 || split.Length == 4)
                {
                    httpClientHandler.Proxy = new WebProxy()
                    {
                        Address = new Uri($"http://{split[0]}:{split[1]}"),
                    };
                    httpClientHandler.UseCookies = false;
                    httpClientHandler.UseProxy = true;
                    if (split.Length == 4)
                        httpClientHandler.DefaultProxyCredentials = new NetworkCredential(split[2], split[3]);
                }
                else
                {
                    logger.LogInformation($"Proxy sai định dạng: {proxy}");
                    return false;//wrong proxy
                }
            }
            using HttpClient httpClient = new HttpClient(httpClientHandler, false);

            if (!await httpClient.TestProxyLiveAsync(cancellationToken))
            {
                logger.LogInformation($"Proxy '{proxy}' chết");
                return false;
            }
            return true;
        }
    }
}
