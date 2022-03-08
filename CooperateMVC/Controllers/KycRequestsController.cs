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

namespace CooperateMVC.Controllers
{
    public class KycRequestsController : Controller
    {
        private readonly CooperateMVCContext _context;

        public KycRequestsController(CooperateMVCContext context)
        {
            _context = context;
        }

        // GET: KycRequests
        public async Task<IActionResult> Index()
        {
            var dekoSharp = new DekoSharp();
            var bankList = dekoSharp.RetrieveAllKycRequest(out var statusCode);
            return View(bankList);
        }

        // GET: KycRequests/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kycRequest = new DekoSharp().RetrieveKYCRequestWithRCNumber(id, out var statusCode);
            if (kycRequest == null)
            {
                return NotFound();
            }

            return View(kycRequest);
        }

        // GET: KycRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KycRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RequestId,RCNumber,RequesterName,AssignedLawyerId,TimeRequested,LastTimeUpdated,AdditionalInformation,Status")] KycRequest kycRequest)
        {
            if (ModelState.IsValid)
            {
                var bankUser = new DekoSharp().RetrieveBankUserWithCookie(Request);
                if(bankUser == null)
				{
                    return NotFound();
				}
                new DekoSharp().CreateKycRequest(bankUser.BankName, kycRequest.RCNumber, kycRequest.AdditionalInformation, out var statusCode);
                return RedirectToAction(nameof(Index));
            }
            return View(kycRequest);
        }

        // GET: KycRequests/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var kycRequest = new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode);
            if (kycRequest == null)
            {
                return NotFound();
            }
            return View(kycRequest);
        }

        // POST: KycRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,RequestId,RCNumber,RequesterName,AssignedLawyerId,TimeRequested,LastTimeUpdated,AdditionalInformation,Status")] KycRequest kycRequest)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var context = HttpContext;
                    new DekoSharp().UpdateDataToPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(kycRequest.RCNumber))}", kycRequest, out var statusCode);
                    if (statusCode != System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("statusCode", "" + statusCode);
                        return View(ModelState);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!KycRequestExists(kycRequest.RCNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ExceptionHandler.HandleException(ex);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(kycRequest);
        }

        // GET: KycRequests/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kycRequest = new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode);
            if (kycRequest == null)
            {
                return NotFound();
            }

            return View(kycRequest);
        }

        // POST: KycRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var kycRequest = new DekoSharp().DeleteKycRequest(id, out var statusCode);
                return RedirectToAction(nameof(Index));
            }
			catch(Exception ex)
			{
                ExceptionHandler.HandleException(ex);
                return View();
			}
        }

        private bool KycRequestExists(string id)
        {
            return new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode) == null;
        }
    }
}
