using Jason5Lee.PropertyCharge.Boundary;
using Jason5Lee.PropertyCharge.Views.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Controllers
{
    public class AccountController : Controller
    {
        private readonly PropertyChargeDbContext _context;
        private readonly AdminContext _adminContext;
        private readonly IPwdHasher _hasher;

        public AccountController(PropertyChargeDbContext context, AdminContext adminContext, IPwdHasher hasher)
        {
            _context = context;
            _adminContext = adminContext;
            _hasher = hasher;
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult Personale()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string role, string personaleIdString, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(role))
            {
                return BadRequest($"\"{nameof(role)}\" cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest($"\"{nameof(password)}\" cannot be null or empty");
            }

            if (role == "personale")
            {
                if (string.IsNullOrEmpty(personaleIdString))
                {
                    ViewData["FailedMessage"] = "身份证号不可为空";
                    return View("Personale");
                }
                if (!ulong.TryParse(personaleIdString, out ulong personaleId))
                {
                    ViewData["FailedMessage"] = "身份证号不合法";
                    return View("Personale");
                }
                if ((await _context.Personales.FindAsync(personaleId))?.Password != _hasher.Hash(password))
                {
                    ViewData["FailedMessage"] = "身份证号或密码错误";
                    return View("Personale");
                }
                var claims = new List<Claim>
                    {
                        new Claim("role", "personale"),
                        new Claim("id", personaleIdString),
                    };
                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "id", "role")));
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return Redirect("/personale");
            }
            else if (role == "admin")
            {
                if (_adminContext.TryLogin(password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim("role", "admin"),
                    };
                    await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", null, "role")));
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return Redirect("/admin");

                }
                else
                {
                    return View("Admin", new AdminModel
                    {
                        LoginFailedMessage = "密码错误"
                    });
                }
            }
            else
            {
                return BadRequest($"unknown login role: \"{role}\"");
            }
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
