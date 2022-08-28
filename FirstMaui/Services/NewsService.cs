using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Concurrent;
using System.Threading.Tasks;

using FirstMaui.Models;

namespace FirstMaui.Services
{
    public class NewsService
    {
        HttpClient httpClient;
        readonly string apiKey = "d318329c40734776a014f9d9513e14ae";

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        ConcurrentDictionary<string, NewsGroup> cachedNews = new ConcurrentDictionary<string, NewsGroup>();

        public event EventHandler<string> NewsAvailable;
        protected virtual void OnNewsAvailable(string message)
        {
            NewsAvailable?.Invoke(this, message);
        }
        public async Task<NewsGroup> GetNewsAsync(NewsCategory category)
        {
            NewsCacheKey key = new NewsCacheKey(category, DateTime.Now);
            if (key.CacheExist)
            {
                OnNewsAvailable($"XML Cached news in category is available: {category}");
                return NewsGroup.Deserialize(key.FileName);
            }

            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}";

             // make the http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            //Convert Json to Object
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            var news = new NewsGroup()
            {
                Category = category,
                Articles = nd.Articles.Select(ndi => new NewsItem()
                {
                    DateTime = ndi.PublishedAt,
                    Title = ndi.Title,
                    Url = ndi.Url,
                    UrlToImage = ndi.UrlToImage
                }).ToList()
            };

            NewsGroup.Serialize(news, key.FileName);
            OnNewsAvailable($"News in category is available: {category}");
            return news;
        }
    }
}
