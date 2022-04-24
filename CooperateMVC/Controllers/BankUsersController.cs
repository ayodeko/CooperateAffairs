using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CooperateMVC.Data;
using CooperateMVC.Models;
using CooperateMVC.Logic;
using Newtonsoft.Json;
using CooperateMVC.ViewModels;

namespace CooperateMVC.Controllers
{
    [ClaimRequirement("", "")]
    public class BankUsersController : Controller
    {
        private readonly CooperateMVCContext _context;

        public BankUsersController(CooperateMVCContext context)
        {
            _context = context;
        }

        // GET: BankUsers
        public async Task<IActionResult> Index()
        {
            var dekoSharp = new DekoSharp();
            var bankList = dekoSharp.RetrieveAllBanksFromTable(out var statusCode);
            return View(bankList);
        }

        // GET: BankUsers/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string? uid)
        {

            if (uid == null)
            {
                return NotFound();
            }

            var bankUser = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
                if (bankUser == null)
                {
                    return NotFound();
                }

            return View(bankUser);
        }

        // GET: BankUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BankUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BankName,BankOfficerName,BankOfficerPhone,BankAuthToken,Id,Email,PhoneNumber,DisplayName,Password,EmailVerified,Role")] BankUser bankUser)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    new DekoSharp().CreateBankTable(bankUser, out var statusCode);
                    if (statusCode != System.Net.HttpStatusCode.OK)
                    {
                        return new StatusCodeResult(((int)statusCode));
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest();
					throw;
				}
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: BankUsers/Edit/5
        public async Task<IActionResult> Edit(string uid)
        {
            if (uid == null)
            {
                return NotFound();
            }

            var bankUser = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
            if (bankUser == null)
            {
                return NotFound();
            }
            return View(bankUser);
        }

        // POST: BankUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBankUserViewModel editBankUser)
        {

            //Use uid from cookie to retrieve user details
            if (string.IsNullOrEmpty(editBankUser.Uid))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (BankUserExists(editBankUser.Uid) == null)
                    {
                        return NotFound();
                    }
                    Console.WriteLine("Bank User: " + JsonConvert.SerializeObject(editBankUser));
                    new DekoSharp().UpdateDataToPath($"test/banks/{DekoUtility.CheckNullOrEmpty(editBankUser.Uid)}", DekoUtility.RemoveNulls(editBankUser), out var statusCode);
                    if(statusCode != System.Net.HttpStatusCode.OK)
					{
                        return new StatusCodeResult(((int)statusCode));
					}
                }
                catch (Exception ex)
                {
                    DekoUtility.LogError(ex);
                        throw ex;
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editBankUser);
        }

        // GET: BankUsers/Delete/5
        public async Task<IActionResult> Delete(string uid)
        {
            if (uid == null)
            {
                return NotFound();
            }

            var bankUser = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
            if (bankUser == null)
            {
                return NotFound();
            }

            return View(bankUser);
        }

        // POST: BankUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (BankUserExists(uid) == null)
                    {
                        return NotFound();
                    }
                    new DekoSharp().DeleteBankUser(uid, out var statusCode);
                    if (statusCode != System.Net.HttpStatusCode.OK)
                    {
                        return new StatusCodeResult(((int)statusCode));
                    }
                }
                catch (Exception ex)
                {
                    throw;

                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private BankUser BankUserExists(string id)
        {
            return new DekoSharp().RetrieveBankFromTable(id, out var statusCode);
        }
    }
}
