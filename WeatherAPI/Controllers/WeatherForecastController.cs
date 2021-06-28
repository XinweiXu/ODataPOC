using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using WeatherAPI.Models.Weather;
using Microsoft.AspNetCore.Mvc;

namespace WeatherAPI.Controllers
{
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [EnableQuery]
        public IQueryable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index =>
            {
                var weather = new WeatherForecast
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
                if (weather.TemperatureC < 0)
                {
                    var detail1 = new WeatherTypeDetail
                    {
                        Location = "North",
                        Description = "Cold weather in north."
                    };
                    var details = new List<WeatherTypeDetail>();
                    details.Add(detail1);
                    weather.WeatherCategory = new WeatherCategory
                    {
                        WeatherType = "Cold",
                        WeatherTypeDetails = details
                    };
                }
                else if (weather.TemperatureC > 30)
                {
                    var detail3 = new WeatherTypeDetail
                    {
                        Location = "South",
                        Description = "Hot weather in south."
                    };
                    var details = new List<WeatherTypeDetail>();
                    details.Add(detail3);
                    weather.WeatherCategory = new WeatherCategory
                    {
                        WeatherType = "Hot",
                        WeatherTypeDetails = details
                    };
                }
                else
                {
                    var detail5 = new WeatherTypeDetail
                    {
                        Location = "Middle",
                        Description = "Mild weather in Middle."
                    };
                    var details = new List<WeatherTypeDetail>();
                    details.Add(detail5);
                    weather.WeatherCategory = new WeatherCategory
                    {
                        WeatherType = "Mild",
                        WeatherTypeDetails = details
                    };
                }           
                return weather;
            }).AsQueryable();
        }
    }
}
