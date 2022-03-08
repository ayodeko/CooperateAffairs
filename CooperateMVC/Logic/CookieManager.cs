using Microsoft.AspNetCore.Http;
using System;

namespace CooperateMVC.Logic
{
	public class CookieManager
	{
		HttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

		public void SetCookie(string key, string value, HttpResponse response, int? expireTime = 0)
		{
			CookieOptions option = new CookieOptions();

			if (expireTime.HasValue || expireTime != 0)
			{ option.Expires = DateTime.Now.AddMinutes(expireTime.Value); }
			else option.Expires = DateTime.Now.AddMilliseconds(1000000000);

			response.Cookies.Append(key, value, option);
		}

		public void SetSession(HttpContext context, string sessionKey, string sessionValue)
		{
			context.Session.SetString(sessionKey, sessionValue);
		}

		public string GetCookie(string key, HttpRequest request)
		{
			string cookieValueFromReq = request.Cookies[key];
			return cookieValueFromReq;
		}
		public void DeleteCookie(string key, HttpResponse response)
		{
			response.Cookies.Delete(key);
		}

	}
}
