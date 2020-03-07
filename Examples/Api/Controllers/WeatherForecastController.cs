using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [Route("WeatherForecast")]
    [Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly IList<string> Summaries = new List<string>
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IList<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Count)]
            })
            .ToArray();
        }

        public class AddWeatherForecastModel
        {
            public string WeatherName { get; set; }
        }

        [HttpPost]
        [Authorize(Policy = "edit:weather_forecast")]
        public bool Add([FromBody]AddWeatherForecastModel model)
        {
            if (string.IsNullOrEmpty(model.WeatherName))
                return false;
            Summaries.Add(model.WeatherName);
            return true;
        }
    }
}
