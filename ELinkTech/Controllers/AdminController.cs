using ELinkTech.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ELinkTech.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly DataContext db;

        public AdminController(UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                DataContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
        }
        [HttpGet]
        public IActionResult GetCategories()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpGet]
        public IActionResult EditCategory()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteCategory()
        {
            return View();
        }
    }
}
