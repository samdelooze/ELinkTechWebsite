using ELinkTech.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data.Entity;

namespace ELinkTech.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class CustomerController : Controller
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly DataContext db;

        public CustomerController(UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager,
                DataContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
        }

        [HttpGet]
        public async Task<ActionResult> UserProfile()
        {
            var userID = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userID);

            ApplicationUser d = db.users.Where(user => user.Id == userID).FirstOrDefault();

            if (d != null)
            {
                return View(d);
            }
            else
            {
                return RedirectToAction("Index", "Main");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            ApplicationUser p = await userManager.FindByIdAsync(Id);
            return View(p);
        }

        [HttpPost] // Update
        public async Task<IActionResult> Edit(string Id, string FirstName, string LastName, string State, string Address, string Suburb, string Postcode, DateTime DateOfBirth)
        {
            ApplicationUser p = await userManager.FindByIdAsync(Id);
            if (p != null)
            {
                if (!string.IsNullOrEmpty(FirstName))
                    p.FirstName = FirstName;
                else
                    ModelState.AddModelError("", "First Name cannot be empty");

                if (!string.IsNullOrEmpty(LastName))
                    p.LastName = LastName;
                else
                    ModelState.AddModelError("", "Last Name cannot be empty");

                if (!string.IsNullOrEmpty(State))
                    p.State = State;
                else
                    ModelState.AddModelError("", "State cannot be empty");

                if (!string.IsNullOrEmpty(Address))
                    p.Address = Address;
                else
                    ModelState.AddModelError("", "Address cannot be empty");

                if (!string.IsNullOrEmpty(Suburb))
                    p.Suburb = Suburb;
                else
                    ModelState.AddModelError("", "Suburb cannot be empty");

                if (!string.IsNullOrEmpty(Postcode))
                    p.Postcode = Postcode;
                else
                    ModelState.AddModelError("", "Postcode cannot be empty");

                p.DateOfBirth = DateOfBirth;

                if (ModelState.IsValid)
                {
                    IdentityResult result = await userManager.UpdateAsync(p);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Main");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(p);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

