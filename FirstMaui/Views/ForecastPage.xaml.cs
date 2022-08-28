using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using FirstMaui.Models;
using FirstMaui.Services;

namespace FirstMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ForecastPage : ContentPage
    {
        OpenWeatherService service;
        GroupedForecast groupedforecast;
        string City;


        public ForecastPage()
        {
            InitializeComponent();
            
            service = new OpenWeatherService();
            groupedforecast = new GroupedForecast();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            City = "Stockholm";
            Title = $"Forecast for {City}";

            MainThread.BeginInvokeOnMainThread(async () => {await LoadForecast();});
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await LoadForecast();
        }

        private async Task LoadForecast()
        {
            Forecast forecast = await service.GetForecastAsync(City);

            groupedforecast.City = forecast.City;
            groupedforecast.Items = forecast.Items.GroupBy(item => item.DateTime.Date);
            GroupedForecast.ItemsSource = groupedforecast.Items;
        }
    }
}