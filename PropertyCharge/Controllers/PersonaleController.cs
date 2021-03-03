using Jason5Lee.PropertyCharge.Boundary;
using Jason5Lee.PropertyCharge.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Controllers
{
    [Authorize(Roles = "personale")]
    public class PersonaleController : Controller
    {
        /// <summary>
        /// Exception when current login personale is deleted.
        /// </summary>
        private class PersonaleDeletedException : Exception { }
        private readonly PropertyChargeDbContext _context;
        private readonly IPwdHasher _pwdHasher;

        public PersonaleController(PropertyChargeDbContext context, IPwdHasher pwdHasher)
        {
            _context = context;
            _pwdHasher = pwdHasher;
        }

        private async Task<ulong> GetPersonaleId()
        {
            return (await GetPersonale()).Id;
        }

        private async Task<Personale> GetPersonale()
        {
            var id = ulong.Parse(HttpContext.User.FindFirst("id").Value);
            var personale = await _context.Personales.FindAsync(id);
            if (personale == null)
            {
                await HttpContext.SignOutAsync();
                throw new PersonaleDeletedException();
            }
            return personale;
        }

        private IActionResult DeletedAction()
        {
            return Redirect("/Home");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["PersonaleId"] = await GetPersonaleId();
                return View();
            }
            catch (PersonaleDeletedException)
            {
                return DeletedAction();
            }
        }

        public async Task<IActionResult> Bill()
        {
            try
            {
                var id = await GetPersonaleId();
                var charges = await _context.Charges
                    .Where(ch => ch.PersonaleId == id)
                    .Where(ch => !ch.Paid)
                    .ToListAsync();

                return View(new Bill
                {
                    PersonaleId = id,
                    Charges = charges,
                    BillFee = charges.Sum(ch => ch.Fee),
                });
            }
            catch (PersonaleDeletedException)
            {
                return DeletedAction();
            }
        }

        public async Task<IActionResult> Payment()
        {
            try
            {
                var id = await GetPersonaleId();
                var charges = await _context.Charges
                    .Where(ch => ch.PersonaleId == id && ch.Paid)
                    .ToListAsync();

                ViewData["PersonaleId"] = id;
                return View(charges);
            }
            catch (PersonaleDeletedException)
            {
                return DeletedAction();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Password()
        {
            try
            {
                await GetPersonaleId();
                return View();
            }
            catch (PersonaleDeletedException)
            {
                return DeletedAction();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Password(string password, string confirmPassword)
        {
            try
            {
                var personale = await GetPersonale();

                if (password != confirmPassword)
                {
                    ViewData["FailedMessage"] = "两次输入密码不一致";
                    return View();
                }

                personale.Password = _pwdHasher.Hash(password);
                _context.Personales.Update(personale);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (PersonaleDeletedException)
            {
                return DeletedAction();
            }
        }
    }
}
