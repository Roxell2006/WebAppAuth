using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp
{
    public class Constants
    {
        public static string BaseUrl = "https://192.168.0.11:45455/";
        public static string Url = BaseUrl + "api/Authenticate";
        public static string UrlTemp = BaseUrl + "WeatherForecast";
    }
}
