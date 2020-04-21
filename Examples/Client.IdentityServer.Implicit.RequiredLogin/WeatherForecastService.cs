using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.IdentityServer.Implicit.RequiredLogin
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
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF { get; set; }
    }
}
