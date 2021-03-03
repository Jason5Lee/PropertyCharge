using Jason5Lee.PropertyCharge.Boundary;
using Jason5Lee.PropertyCharge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Controllers
{
    [Authorize(Roles = "admin")]
    public class BillManagementController : Controller
    {
        private readonly PropertyChargeDbContext _context;
        public BillManagementController(PropertyChargeDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Query(string personaleIdString)
        {
            if (string.IsNullOrEmpty(personaleIdString))
            {
                ViewData["FailedMessage"] = "身份证号不能为空";
                return View("Index");
            }
            if (!ulong.TryParse(personaleIdString, out var personaleId))
            {
                ViewData["FailedMessage"] = "身份证号不合法";
                return View("Index");
            }
            if (await _context.Personales.FindAsync(personaleId) == null)
            {
                ViewData["FailedMessage"] = "找不到用户";
                return View("Index");
            }

            var charges = await _context.Charges
                .Where(ch => ch.PersonaleId == personaleId)
                .Where(ch => !ch.Paid)
                .ToListAsync();

            return View(new Bill
            {
                PersonaleId = personaleId,
                Charges = charges,
                BillFee = charges.Sum(ch => ch.Fee),
            });
        }

        [HttpGet]
        public IActionResult CreateCharge(string personaleIdString)
        {
            if (string.IsNullOrEmpty(personaleIdString))
            {
                return BadRequest($"\"{nameof(personaleIdString)}\" cannot be null");
            }
            if (!ulong.TryParse(personaleIdString, out var personaleId))
            {
                return BadRequest($"\"{personaleIdString}\" 不是合法的身份证号");
            }
            ViewData["PersonaleId"] = personaleId;
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCharge([Bind("Id,PersonaleId,Name,Paid,Fee")] Charge charge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(charge);
                await _context.SaveChangesAsync();
                return RedirectToQuery(charge);
            }
            ViewData["FailedMessage"] = ModelState.First().Value;
            return View();
        }

        private RedirectToActionResult RedirectToQuery(ulong personaleId)
        {
            return RedirectToAction("Query", new
            {
                PersonaleIdString = personaleId.ToString(),
            });
        }

        private RedirectToActionResult RedirectToQuery(Charge charge)
        {
            return RedirectToQuery(charge.PersonaleId);
        }

        [HttpGet]
        public async Task<IActionResult> EditCharge(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charge = await _context.Charges.FindAsync(id);
            if (charge == null)
            {
                return NotFound();
            }
            ViewData["PersonaleId"] = id;
            return View(charge);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCharge(int id, [Bind("Id,PersonaleId,Name,Paid,Fee")] Charge charge)
        {
            if (id != charge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChargeExists(charge.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToQuery(charge);
            }
            ViewData["PersonaleId"] = id;
            return View(charge);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCharge(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charge = await _context.Charges
                .Include(c => c.Personale)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charge == null)
            {
                return NotFound();
            }

            return View(charge);
        }

        [HttpPost, ActionName("DeleteCharge")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChargeConfirmed(int id)
        {
            var charge = await _context.Charges.FindAsync(id);
            _context.Charges.Remove(charge);
            await _context.SaveChangesAsync();
            return RedirectToQuery(charge);
        }

        [HttpGet]
        public IActionResult PayConfirm(ulong personaleId, decimal billFee, string chargeIds)
        {
            ViewData["BillFee"] = billFee;
            ViewData["ChargeIds"] = chargeIds;
            ViewData["PersonaleId"] = personaleId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayConfirm(ulong personaleId, string chargeIds)
        {
            var ids = chargeIds.Split(',')
                .Select(str => int.Parse(str))
                .ToList();

            _context.Charges.UpdateRange(
                _context.Charges.Where(ch => ids.Contains(ch.Id))
                    .Select(ch => new Charge
                    {
                        Id = ch.Id,
                        PersonaleId = ch.PersonaleId,
                        Name = ch.Name,
                        Fee = ch.Fee,
                        Paid = true,
                    }));
            await _context.SaveChangesAsync();
            return RedirectToQuery(personaleId);
        }

        private bool ChargeExists(int id)
        {
            return _context.Charges.Any(e => e.Id == id);
        }
    }
}
