using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.IdentityServer.Code.Complex
{
    public class WeatherForecastService
    {
        private HttpClient _httpClient;
        private readonly IAuthenticationStateProvider _stateProvider;

        public WeatherForecastService(HttpClient httpClient, IAuthenticationStateProvider stateProvider)
        {
            _httpClient = httpClient;
            _stateProvider = stateProvider;
        }

        public async Task<IList<WeatherForecast>> GetAll()
        {
            await _stateProvider.SetAuthorizationHeader(_httpClient);
            return await _httpClient.GetFromJsonAsync<IList<WeatherForecast>>("WeatherForecast");
        }

        public async Task<bool> Create(AddWeatherForecastModel model)
        {
            await _stateProvider.SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.PostAsJsonAsync("WeatherForecast", model);
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF { get; set; }
    }

    public class AddWeatherForecastModel
    {
        public string WeatherName { get; set; }
    }
}
