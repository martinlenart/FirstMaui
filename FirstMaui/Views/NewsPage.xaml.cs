using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

using FirstMaui.Models;
using FirstMaui.Services;

namespace FirstMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        NewsService service;
        NewsGroup Headlines;
        NewsCategory category;

        public NewsPage()
        {
            InitializeComponent();
            this.category = NewsCategory.business;

            service = new NewsService();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainThread.BeginInvokeOnMainThread(async () => { await LoadNews(); });
            newsHeader.Text = $"Todays {this.category} Headlines";
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await LoadNews();
        }

        private async Task LoadNews()
        {
            Headlines = await service.GetNewsAsync(category);
            newsHeadlines.ItemsSource = Headlines.Articles;
        }

        private void newsHeadlines_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedArticle = e.Item as NewsItem;
            var url = HttpUtility.UrlEncode(selectedArticle.Url);

            Navigation.PushAsync(new ArticleView(url));
        }
    }
}