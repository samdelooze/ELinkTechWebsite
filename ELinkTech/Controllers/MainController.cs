using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ELinkTech.Models;
using Microsoft.AspNetCore.Identity;
using ELinkTech.ViewModels;
using Microsoft.Win32;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ELinkTech.Controllers;

public class MainController : Controller

{
    private UserManager<ApplicationUser> userManager { get; }
    private SignInManager<ApplicationUser> signInManager { get; }
    private RoleManager<IdentityRole> roleManager { get; }

    private IEmailSender emailSender;

    private readonly DataContext db;

    public MainController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            DataContext db)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.emailSender = emailSender;
        this.db = db;
    }
    [HttpGet]
    public async Task<IActionResult> Index(Main m)
    {

        await SeedData.SeedAsync(userManager, roleManager);
        var userID = HttpContext.Session.GetString("userid");
        var getProduct = from products in db.products select products;
           
        Quote? quote = new Quote() {
            UserID = userID,
            UserEmail = User.Identity?.Name!
        };
        RetrieveProducts(getProduct, quote);

        var product = from products in db.products
                      join suppliers in db.suppliers
                      on products.SupplierID equals suppliers.SupplierID
                      join categories in db.categories
                      on products.CategoryID equals categories.CategoryID
                      select new
                      {
                          ProductID = products.ProductID,
                          ProductName = products.ProductName,
                          SupplierName = suppliers.SupplierName,
                          CategoryName = categories.CategoryName
                      };

        List<Product> productList = new List<Product>();

        foreach (var products in product)
        {
            productList.Add(new Product
            {
                ProductID = products.ProductID,
                ProductName = products.ProductName,
                SupplierID = products.SupplierName,
                CategoryID = products.CategoryName
            });
        }
        SubmitQuote(m);
        m.product = productList;
        return View(m);
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpPost]
    public async Task<IActionResult> Login(Main m)//login the user
    {
        if (ModelState.IsValid)
        {
            var userEmail = await userManager.FindByEmailAsync(m.LoginEmail);
            if (userEmail != null && !userEmail.EmailConfirmed &&
                (await userManager.CheckPasswordAsync(userEmail, m.LoginPassword)))
            {
                //ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                TempData["AlertFail"] = "Email not confirmed yet";
                return RedirectToAction("Index");
            }

            var result = await signInManager.PasswordSignInAsync(m.LoginEmail, m.LoginPassword, m.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(m.LoginEmail);
                var userId = user.Id;
                HttpContext.Session.SetString("userid", userId);

                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Username or Password is incorrect");
        }
        TempData["AlertFail"] = "Invalid Login Attempt. Try again";
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Register(Main m)//create an account
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = m.RegisterEmail,
                Email = m.RegisterEmail,
                FirstName = m.FirstName,
                LastName = m.LastName,
                PhoneNumber = m.ContactNumber,
                State = m.State,
                Address = m.Address,
                Suburb = m.Suburb,
                Postcode = m.Postcode,
                DateOfBirth = m.DateOfBirth
            };
            var result = await userManager.CreateAsync(user, m.RegisterPassword);

            if (result.Succeeded)
            {
                // Confirmation email start
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var EmailConfirmationUrl = Url.Action("ConfirmEmail", "Main", new { userId = user.Id, token = token }, Request.Scheme);

                await emailSender.SendEmailAsync(m.RegisterEmail, "[ELinkTech] Welcome to ELinkTech! Confirm your email", "Please confirm your email by clicking <a href =\"" + EmailConfirmationUrl + "\">here</a> so you can start using ELinkTech web service.");

                TempData["AlertSuccess"] = "Registration is successful. Confirmation link was sent to your email address(" + m.RegisterEmail + "). Please check and verify it.";
                // Confirmation email end


                //await userManager.AddToRoleAsync(user, "User");
                //await signInManager.SignInAsync(user, false); // Restrict login before user comfirm the email
                return View("Index");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

        }
        return View("Index");
    }
    [HttpGet]
    //[AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            TempData["AlertFail"] = "Invalid trial";
            return View("Index");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["AlertFail"] = "Invalid trial";
            return View("Index");
        }

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            TempData["AlertSuccess"] = "Email verification is successful. You can login now";
            return View("Index");
        }
        TempData["AlertFail"] = "Fail to verify your email";
        return View("Index");
    }

    [HttpGet]
    public IActionResult Logout()//open logout view
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> LogoutYes()//sign out user
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult LogoutNo()//redirect to home
    {
        return RedirectToAction("Index");
    }
}

<<<<<<< Updated upstream
/*    [HttpGet]*/
/*    public IActionResult Quotes(string id)
    {
        var product = from products in db.products select products;

        var category = from categories in db.categories select categories;
        Quote? quote = db.quotes.Where(p => p.ProductID == id).FirstOrDefault();

        RetrieveSuppliers(supplier, product);
        RetrieveCategories(category, product);
        return View(product);
    }*/
=======
    [HttpGet]
    public IActionResult SubmitQuote(Main m)
    {
        var userID = HttpContext.Session.GetString("userid");
        var getProduct = from products in db.products select products;

        Quote? quote = new Quote()
        {
            UserID = userID,
            UserEmail = User.Identity?.Name!
        };
        RetrieveProducts(getProduct, quote);
        m.quote = quote;
        return View(m);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitQuote(Quote quote)
    {

        await db.quotes.AddAsync(quote);
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Main");
    }
    private void RetrieveProducts(IQueryable<Product> p, Quote q)
    {
        foreach (var product in p)
        {
            SelectListItem item = new SelectListItem();
            item.Text = product.ProductName;
            item.Value = product.ProductID.ToString();
            q.ProductList.Add(item);
        }
    }
>>>>>>> Stashed changes

