using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using FirstMaui.Models;

namespace FirstMaui.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        readonly string apiKey = "eee86395bdce14b3d962d5956193d800";

        public event EventHandler<string> WeatherForecastAvailable;
        protected virtual void OnWeatherForecastAvailable (string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            if(cachedCityForecasts.ContainsKey((City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                OnWeatherForecastAvailable($"Cached weather forecast for {City} available");
                return cachedCityForecasts[(City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];
            }
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);
            cachedCityForecasts[(City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))] = forecast;
            OnWeatherForecastAvailable($"New weather forecast for {City} available");
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            if (cachedGeoForecasts.ContainsKey((latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                OnWeatherForecastAvailable($"Cached weather forecast for {(latitude, longitude)} available");
                return cachedGeoForecasts[(latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];
            }
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);
            cachedGeoForecasts[(latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))] = forecast;
            OnWeatherForecastAvailable($"New weather forecast for {(latitude, longitude)} available");
            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            var forecast = new Forecast()
            {
                City = wd.city.name,
                Items = wd.list.Select(wdle => new ForecastItem()
                {
                    DateTime = UnixTimeStampToDateTime(wdle.dt),
                    Temperature = wdle.main.temp,
                    WindSpeed = wdle.wind.speed,
                    Description = wdle.weather.First().description,
                    Icon = $"http://openweathermap.org/img/w/{wdle.weather.First().icon}.png"
                }).ToList()
            };
            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
