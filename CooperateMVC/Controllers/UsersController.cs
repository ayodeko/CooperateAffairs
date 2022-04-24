using CooperateMVC.Logic;
using CooperateMVC.Models;
using CooperateMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CooperateMVC.Controllers
{
	public class UsersController : Controller
	{
        Dekobase _dekobase = new Dekobase();

        /*
        public IActionResult BankSignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BankSignUp([Bind("BankName,BankOfficerName,BankOfficerPhone,BankAuthToken,Id,Email,PhoneNumber,DisplayName,Password,EmailVerified,Role")] BankUser bankUser)
		{
            
            if (ModelState.IsValid)
            {
                var responseString = _dekobase.CreateBankUser(HttpContext, bankUser);

                if (responseString.StartsWith("Exception"))
                {
                    handleFailure();
                    return View();
                }
                
                return RedirectToAction("Home", "Index");
            }
            return View(bankUser);
		}
        */

        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult BankSignIn()
        {
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        public IActionResult BankSignIn(UserLoginViewModel model)
		{
            try
            {
                if (ModelState.IsValid)
                {
                    var responseString = _dekobase.SignInUserAsync(HttpContext, model.Email, model.Password);

                    if (responseString.Result.StartsWith("Exception"))
                    {
                        handleException(new Exception(responseString.Result));
                        return View();
                    }

                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch(Exception ex)
			{
                ModelState.AddModelError("", ex.Message);
                return View();
			}
		}

        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult BankSignUp()
        {
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        public IActionResult BankSignUp(CreateBankUserViewModel model)
		{
            if (ModelState.IsValid)
            {
                var bankUser = new BankUser
                {
                    BankName = model.BankName,
                    PhoneNumber = model.BankOfficerPhoneNumber,
                    DisplayName = model.BankName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = "bank",
                    BankOfficerName = model.BankOfficerName,
                    BankOfficerPhone = model.BankOfficerPhoneNumber
                };

                var responseString = _dekobase.CreateBankUser(HttpContext, bankUser);

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

        [ClaimRequirement("", "")]
        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult EditBankUserProfile()
        {
            var uid = new CookieManager().GetCookie("_firebaseUserId", Request);
            if (string.IsNullOrEmpty(uid)) { Console.WriteLine($"cookie uid is null or empty"); return NotFound(); }
            var user = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
            if (user == null)
            {
                Console.WriteLine($"retrieved user is null");
                return NotFound();
            }
            var userViewModel = new EditBankUserProfileViewModel
            {
                BankOfficerName = user.BankOfficerName,
                BankName = user.BankName,
                Uid = uid,
                BankOfficerPhoneNumber = user.BankOfficerPhone,
                Email = user.Email,
            };
            return View(userViewModel);
        }

        [ClaimRequirement("", "")]
        [HttpPost]
        //[AllowAnonymous]
        public IActionResult EditBankUserProfile(EditBankUserProfileViewModel model)
		{
            try
            {
                if (ModelState.IsValid)
                {
					if (string.IsNullOrEmpty(model.Uid)) { ModelState.AddModelError("", "Uid cannot be null"); }
                    var bankUser = new BankUser
                    {
                        Uid = model.Uid,
                        BankName = model.BankName,
                        PhoneNumber = model.BankOfficerPhoneNumber,
                        DisplayName = model.BankName,
                        Email = model.Email,
                        Role = "bank",
                        BankOfficerName = model.BankOfficerName,
                        BankOfficerPhone = model.BankOfficerPhoneNumber
                    };

                    Console.WriteLine("Serialize object: "+ JsonConvert.SerializeObject(bankUser));

                    var responseString = _dekobase.UpdateBankUser(HttpContext, bankUser);

                    if (responseString.StartsWith("Exception"))
                    {
                        Console.WriteLine("Update user exception : " + responseString);
                        ModelState.AddModelError("", responseString);
                        return View();
                    }
                    Console.WriteLine("User updated");
                    return RedirectToAction("Index", "BankNavigation");
                }
                Console.WriteLine("ModelState Invalid");
                return View();
            }
            catch(Exception ex)
			{
                handleException(ex);
                throw;
			}
		}

        [ClaimRequirement("", "")]
        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult ChangePassword()
        {
            var uid = new CookieManager().GetCookie("_firebaseUserId", Request);
            if (string.IsNullOrEmpty(uid)) { Console.WriteLine($"cookie uid is null or empty"); return NotFound(); }
            var user = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
            if (user == null)
            {
                Console.WriteLine($"retrieved user is null");
                return NotFound();
            }
            var userViewModel = new ChangePasswordViewModel
            {
                Uid = uid
            };
            return View(userViewModel);
        }

        [ClaimRequirement("", "")]
        [HttpPost]
        //[AllowAnonymous]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
		{
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("", "Confirm Password does not match with password inputted");
                    }
                    var bankUser = new BankUser
                    {
                        Uid = model.Uid,
                        Password = model.Password
                    };

                    var responseString = _dekobase.UpdateBankUser(HttpContext, bankUser);

                    if (responseString.StartsWith("Exception"))
                    {
                        Console.WriteLine("Change password exception : " + responseString);
                        ModelState.AddModelError("", responseString);
                        return View();
                    }
                    Console.WriteLine("Password changed!");
                    return RedirectToAction("Index", "BankNavigation");
                }
                Console.WriteLine("ModelState Invalid");
                return View();
            }
            catch(Exception ex)
			{
                handleException(ex);
                throw;
			}
		}


        [ClaimRequirement("", "")]
        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult EditUserDetails()
        {
            var uid = new CookieManager().GetCookie("_firebaseUserId", Request);
            if(string.IsNullOrEmpty(uid)) { Console.WriteLine($"cookie uid is null or empty");  return NotFound(); }
            var user = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
            if(user == null)
			{
                Console.WriteLine($"retrieved user is null");
                return NotFound();
            }
            var userViewModel = new EditBankUserViewModel
            {
                BankOfficerName = user.BankOfficerName,
                BankName = user.BankName,
                Uid = uid
            };
            return View(userViewModel);
        }

        [ClaimRequirement("", "")]
        [HttpPost]
        public IActionResult EditUserDetails(EditBankUserViewModel model)
		{
			if (ModelState.IsValid)
			{

                var bankUser = new BankUser
                {
                    BankOfficerName = model.BankOfficerName,
                    Uid = model.Uid
                };
                var updateResponse = new DekoSharp().UpdateBankTable(bankUser, out var statusCode);
				if (string.IsNullOrEmpty(updateResponse))
				{
                    ModelState.AddModelError("", $"Http Status code: {statusCode}");
                    return View();
				}
				if (updateResponse.ToLower().StartsWith("exception"))
				{
                    ModelState.AddModelError("", updateResponse);
                    return View();
				}
                return RedirectToAction("Index", "Home");
            }
            return View();

        }

		private void handleException(Exception ex)
		{
            Console.WriteLine(ex);
            ModelState.AddModelError("", ex.Message);
			//throw ex;
		}
	}
}
