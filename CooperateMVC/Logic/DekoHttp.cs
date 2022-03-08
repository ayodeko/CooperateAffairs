using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace CooperateMVC.Logic
{
	public class DekoHttp
	{
		HttpClient _httpClient;

		public static string FirebaseHttpPatch(string relativeUrl, object input)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri("https://fir-dotnet-8146c-default-rtdb.firebaseio.com/");
				relativeUrl = $"{relativeUrl}.json?auth=RtFON0RXT985SRpm9mU6ZNxSedpuVZnQVt9jOjfz";
				var stringContent = new StringContent(JsonConvert.SerializeObject(input));

				var response = httpClient.PatchAsync(relativeUrl, stringContent).Result;
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsStringAsync().Result;
			}
		}
		public static string FirebaseOrderByEqualTo(string relativeUrl, string orderBy, int equalTo)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri("https://fir-dotnet-8146c-default-rtdb.firebaseio.com/");
				relativeUrl = $"{relativeUrl}.json?orderBy=\"{orderBy}\"&equalTo={equalTo}&auth=RtFON0RXT985SRpm9mU6ZNxSedpuVZnQVt9jOjfz";
				var response = httpClient.GetAsync(relativeUrl).Result;
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsStringAsync().Result;
			}
		}
		public static string FirebaseOrderByEqualToString(string relativeUrl, string orderBy, string equalTo)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri("https://fir-dotnet-8146c-default-rtdb.firebaseio.com/");
				relativeUrl = $"{relativeUrl}.json?orderBy=\"{orderBy}\"&equalTo=\"{equalTo}\"&auth=RtFON0RXT985SRpm9mU6ZNxSedpuVZnQVt9jOjfz";
				Console.WriteLine($"URL: {httpClient.BaseAddress}/{relativeUrl}");
				var response = httpClient.GetAsync(relativeUrl).Result;
				try
				{
					
					response.EnsureSuccessStatusCode();
					return response.Content.ReadAsStringAsync().Result;
				}
				catch(Exception ex)
				{
					Console.WriteLine("Exception caught: " + ex);
					throw new Exception("Error response from service: " + response.Content.ReadAsStringAsync().Result, ex);
				}
			}
		}
	}
}
