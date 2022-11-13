using ELinkTech.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data.Entity;
using ELinkTech.ViewModels;
using ELinkTech.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using ELinkTech.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ELinkTech.Controllers
{
    
    public class CustomerController : Controller
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly DataContext db;
        private IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public CustomerController(UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager,
                IEmailSender emailSender,
                IConfiguration configuration,
                DataContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
            this.emailSender = emailSender;
            this.configuration = configuration;
        }

        [Authorize(Roles = "Administrator, User")]
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

        [Authorize(Roles = "Administrator, User")]
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            ApplicationUser p = await userManager.FindByIdAsync(Id);
            return View(p);
        }

        [Authorize(Roles = "Administrator, User")]
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

        [AllowAnonymous]
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> QuoteFormAsync()
        {
            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId);

            Quote quote = new Quote();

            if (user != null)
            {
                quote.UserID = userId;
                quote.UserName = user.FirstName + " " + user.LastName;
            }
            else
            {
                quote.UserID = "";
                quote.UserName = "";
            }
            quote.UserEmail = User.Identity?.Name!;


            var getProduct = from products in db.products select products;

            foreach (var product in getProduct)
            {
                SelectListItem item = new SelectListItem();
                item.Text = product.ProductName;
                item.Value = product.ProductID.ToString();
                quote.ProductList.Add(item);
            }

            return PartialView("QuoteForm", quote);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuote(Quote quote)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await db.quotes.AddAsync(quote);
                    await db.SaveChangesAsync();

                    var product = db.products.Where(m => m.ProductID.ToString() == quote.ProductID).FirstOrDefault();
                    var productName = product.ProductName;

                    var senderEmail = configuration["SendGrid:SenderEmail"];

                    await emailSender.SendEmailAsync(
                          senderEmail,
                          "[ELinkTech] User submitted a quote",
                          "User Information: " + quote.UserName + "(" + quote.UserEmail + ")<br>Quote about: " + productName + "<br>Message: " + quote.Message);

                    TempData["AlertSuccess"] = "Your quote is successfully submitted";
                }
                catch (Exception e)
                {
                    TempData["AlertFail"] = "Fail to submit your quote";
                }
            }
            return RedirectToAction("Index", "Main");
        }
    }
}

