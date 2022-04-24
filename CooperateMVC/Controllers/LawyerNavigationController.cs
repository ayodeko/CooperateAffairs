using CooperateMVC.Logic;
using CooperateMVC.Models;
using CooperateMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CooperateMVC.Controllers
{
	public class LawyerNavigationController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
