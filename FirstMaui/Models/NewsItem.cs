using System;

namespace FirstMaui.Models
{
    public class NewsItem
    {
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
    }
}
