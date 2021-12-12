using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace CooperateAffairs.Extensions
{
	public static class DekoExceptionHandler
	{
		public static void ConfigureDekoExceptionHandler(this IApplicationBuilder applicationBuilder)
		{
			applicationBuilder.UseExceptionHandler(exception =>
			{
				exception.Run(async httpContext =>
			   {
				   httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
				   httpContext.Response.ContentType = "application/json";
				   var httpContextFeatures = httpContext.Features.Get<IExceptionHandlerFeature>();

				   if (httpContextFeatures != null)
				   {
					   await httpContext.Response.WriteAsJsonAsync(new DekoExceptionResponse
					   {
						   StatusCode = httpContext.Response.StatusCode,
						   ErrorMessage = $"Exception: {httpContextFeatures.Error}",
						   ResponseString = $"{httpContextFeatures.Error.Message}",
						   ResponseCode = "06"
					   });
				   }
			   });
			});
		}
	}

	public class DekoExceptionResponse
	{
		public string? ResponseCode { get; set; }
		public int? StatusCode { get; set; }
		public string? ResponseString { get; set; }
		public string? ErrorMessage { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
