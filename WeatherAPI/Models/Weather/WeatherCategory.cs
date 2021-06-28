using System.Collections.Generic;

namespace WeatherAPI.Models.Weather
{
    public class WeatherCategory
    {
        public string WeatherType { get; set; }

        public ICollection<WeatherTypeDetail> WeatherTypeDetails { get; set; }
    }
}
