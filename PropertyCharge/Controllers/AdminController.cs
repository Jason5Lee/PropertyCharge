using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Jason5Lee.PropertyCharge.Boundary;

namespace Jason5Lee.PropertyCharge.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AdminContext _adminContext;

        public AdminController(AdminContext adminContext)
        {
            _adminContext = adminContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Password()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Password(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewData["FailedMessage"] = "两次输入密码不一致";
                return View();
            }
            _adminContext.Set(password);
            return RedirectToAction("Index");
        }
    }
}
