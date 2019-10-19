namespace Orc.NuGetExplorer.Web
{
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpSource
    {
        static HttpClient HttpClient;

        // Only one thread may re-create the http client at a time.
        private readonly SemaphoreSlim _httpClientLock = new SemaphoreSlim(1, 1);

        public async Task EnsureHttpClientAsync()
        {
            // Create the http client on the first call
            if (HttpClient == null)
            {
                await _httpClientLock.WaitAsync();

                try
                {
                    // Double check
                    if (HttpClient == null)
                    {
                        HttpClient = await CreateHttpClientAsync();
                    }
                }
                finally
                {
                    _httpClientLock.Release();
                }
            }
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var httpClient = new HttpClient(new HttpClientHandler())
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            // Set accept-language header
            string acceptLanguage = CultureInfo.CurrentUICulture.ToString();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(acceptLanguage);
            }

            return httpClient;
        }
    }
}
