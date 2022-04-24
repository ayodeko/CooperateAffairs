using CooperateMVC.Logic;
using CooperateMVC.Models;
using CooperateMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CooperateMVC.Controllers
{
	public class LawyerController : Controller
	{
        Dekobase _dekobase = new Dekobase();
        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult LawyerSignUp()
        {
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        public IActionResult LawyerSignUp(CreateLawyererViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lawyerBiodata = new LawyerBioData
                {
                    FirmName = model.FirmName,
                    Email = model.Email,
                    DisplayName = model.FirmName,
                    Password = model.Password,
                    PhoneNumber = model.LawyerPhoneNumber,
                    ContactName = model.FirmContactName,
                    Role = "lawyer",
                    FullAddress = model.FullAddress,
                    City = model.City,
                    State = model.State
                };

                var responseString = _dekobase.CreateLawyer(lawyerBiodata);

                if (responseString.StartsWith("Exception"))
                {
                    Console.WriteLine("Create user exception : " + responseString);
                    ModelState.AddModelError("", responseString);
                    return View();
                }
                Console.WriteLine("User created");
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine("ModelState Invalid");
            return View();
        }

        
    }
}
