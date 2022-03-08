using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CooperateMVC.Data;
using CooperateMVC.Models;

namespace CooperateMVC.Controllers
{
    [Route("[controller]/[action]")]
    public class CreateKycRequestsController : Controller
    {
        private readonly CooperateMVCContext _context;

        public CreateKycRequestsController(CooperateMVCContext context)
        {
            _context = context;
        }

        
        // GET: CreateKycRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreateKycRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RCNumber,RequesterName,AdditionalInformation")] CreateKycRequest createKycRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(createKycRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createKycRequest);
        }

        // GET: CreateKycRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var createKycRequest = await _context.CreateKycRequest.FindAsync(id);
            if (createKycRequest == null)
            {
                return NotFound();
            }
            return View(createKycRequest);
        }

        // POST: CreateKycRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RCNumber,RequesterName,AdditionalInformation")] CreateKycRequest createKycRequest)
        {
            if (id != createKycRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(createKycRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreateKycRequestExists(createKycRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(createKycRequest);
        }

        // GET: CreateKycRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var createKycRequest = await _context.CreateKycRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (createKycRequest == null)
            {
                return NotFound();
            }

            return View(createKycRequest);
        }

        // POST: CreateKycRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var createKycRequest = await _context.CreateKycRequest.FindAsync(id);
            _context.CreateKycRequest.Remove(createKycRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreateKycRequestExists(int id)
        {
            return _context.CreateKycRequest.Any(e => e.Id == id);
        }
    }
}
