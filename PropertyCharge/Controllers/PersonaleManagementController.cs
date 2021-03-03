using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Jason5Lee.PropertyCharge.Boundary;
using Jason5Lee.PropertyCharge.Models;
using Microsoft.AspNetCore.Authorization;

namespace Jason5Lee.PropertyCharge.Controllers
{
    [Authorize(Roles = "admin")]
    public class PersonaleManagementController : Controller
    {
        private readonly PropertyChargeDbContext _context;
        private readonly IPwdHasher _pwdHasher;

        public PersonaleManagementController(PropertyChargeDbContext context, IPwdHasher pwdHasher)
        {
            _context = context;
            _pwdHasher = pwdHasher;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Personales.ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string idString, string name, string address, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(idString))
            {
                return BadRequest($"\"{nameof(idString)}\" cannot be null or empty");
            }

            if (!ulong.TryParse(idString, out ulong id))
            {
                return BadRequest($"illegal personale id \"{id}\"");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest($"\"{nameof(name)}\" cannot be null or empty");
            }

            if (string.IsNullOrEmpty(address))
            {
                return BadRequest($"\"{nameof(address)}\" cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest($"\"{nameof(password)}\" cannot be null or empty");
            }

            if (password != confirmPassword)
            {
                ViewData["FailedMessage"] = "两次密码不一致";
                return View();
            }

            try
            {
                _context.Add(new Personale
                {
                    Id = id,
                    Name = name,
                    Address = address,
                    Password = _pwdHasher.Hash(password),
                });


                await _context.SaveChangesAsync();
            } 
            catch (DbUpdateException outer)
            {
                if (outer.InnerException is Npgsql.PostgresException e
                    && e.ConstraintName == "PK_Personales")  // Primary key error, which means the Id is already exists.
                {
                    ViewData["FailedMessage"] = $"该身份证号已存在";
                    return View();
                }
                else
                {
                    throw;
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Personale/Delete/5
        public async Task<IActionResult> Delete(ulong? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personale = await _context.Personales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personale == null)
            {
                return NotFound();
            }

            return View(personale);
        }

        // POST: Personale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ulong id)
        {
            var personale = await _context.Personales.FindAsync(id);
            _context.Personales.Remove(personale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaleExists(ulong id)
        {
            return _context.Personales.Any(e => e.Id == id);
        }
    }
}
