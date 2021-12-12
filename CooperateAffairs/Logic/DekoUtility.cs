using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DotNetFirebase.Logic
{
    public class DekoUtility
    {
        private HttpClient _httpClient = new HttpClient();

        HttpResponseMessage HttpPost(string baseAddress)
        {
            var uri = new UriBuilder(baseAddress);
            //var relativeUrl = $"?key={ConfigurationManager.AppSettings["FirebaseAPIKey"]}";
            var relativeUrl = $"?key=";

            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var data = new
            {
                token = "",
                returnSecureToken = ""
            };
            using (var httpResponse = _httpClient.PostAsync(relativeUrl, new StringContent(JsonConvert.SerializeObject(data))).Result)
            {
                if (httpResponse.Content != null)
                {
                    var responseString = httpResponse.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    var responseString = "Content is null";
                }
            }

            return new HttpResponseMessage();
        }

        public static string ReplaceSpace(string message) => message.Replace(' ', '_');

        public static void LogInfo(string message)
        {
            
        }

        public static void LogError(Exception ex)
        {
            
        }
    }
}