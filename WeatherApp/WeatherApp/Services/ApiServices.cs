using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebAppAuth.Models;

namespace WeatherApp.Services
{
    public class ApiServices
    {
        HttpClient httpClient;

        //Constructeur
        public ApiServices()
        {
#if DEBUG
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (o, cert, chain, errors) => true
            };
#else
			var httpHandler = new HttpClientHandler();
#endif
            httpClient = new HttpClient(httpHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.MaxResponseContentBufferSize = 256000;
        }
        public async Task<string>LoginUserAsync(string url, AuthUserDto data)
        {
            var user = await HttpPostAsync(url, data);

            if (user != null)
            {
                CurrentpropertiesService.SaveUser(user);
                return user.Token;
            }
            else
                return string.Empty;
        }
        public async Task<AuthUserDto>RegisterUserAsync(string url, AuthUserDto data)
        {
            var user = await HttpPostAsync(url, data);
            if (user != null)
                CurrentpropertiesService.SaveUser(user);
            return user;
        }

        public async Task<List<WeatherForecast>> GetTempAsync(string url, string token)
        {
            var data = await HttpGetAsync<WeatherForecast>(url, token);
            return data;
        }

        // Méthode Get Générique
        private async Task<List<T>> HttpGetAsync<T>(string url, string token)
        {
            List<T> result = new List<T>();

            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(url);
                HttpContent content = response.Content;

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<T>>(jsonResponse);
                }
                else
                {
                    throw new Exception(((int)response.StatusCode).ToString() + " - " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                OnError(ex.ToString());
            }

            return result;
        }

        // Méthode Post Générique
        private async Task<T> HttpPostAsync<T>(string url, T data)
        {
            T result = default(T); // résultat de type générique

            try
            {
                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(new Uri(url), content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(jsonResponse);
                }
                else
                {
                    throw new Exception(((int)response.StatusCode).ToString() + " - " + response.ReasonPhrase);
                }
                return result;
            }
            catch (Exception ex)
            {
                OnError(ex.ToString());
                return result;
            }
        }
        private void OnError(string error)
        {
            Console.WriteLine("[WEBSERVICE ERROR] " + error);
        }

    }
}
