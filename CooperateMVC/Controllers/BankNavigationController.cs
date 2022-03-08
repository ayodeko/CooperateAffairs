using CooperateMVC.Logic;
using Microsoft.AspNetCore.Mvc;

namespace CooperateMVC.Controllers
{
	public class BankNavigationController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Logout(string id)
		{
			return View();
		}
		[HttpPost]
		public IActionResult Logout()
		{
			new Dekobase().CookieSignOut(HttpContext);
			return RedirectToAction("Index", "Home");
		}
	}
}
