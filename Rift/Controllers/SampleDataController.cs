using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace Rift.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        static readonly String[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts(Int32 startDateIndex)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public String DateFormatted { get; set; }
            public Int32 TemperatureC { get; set; }
            public Int32 TemperatureF => 32 + (Int32) (TemperatureC / 0.5556);
            public String Summary { get; set; }
        }
    }
}