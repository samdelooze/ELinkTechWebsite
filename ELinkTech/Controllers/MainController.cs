using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ELinkTech.Models;
using Microsoft.AspNetCore.Identity;
using ELinkTech.ViewModels;
using Microsoft.Win32;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

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

        var getProduct = from products in db.products select products;

        var userId = userManager.GetUserId(User);
        var user = await userManager.FindByIdAsync(userId);

        Quote? quote = new Quote();

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
                          ProductImage = products.ProductImage,
                          ProductDetails = products.ProductDetails,
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
                ProductImage = products.ProductImage,
                ProductDetails = products.ProductDetails,
                SupplierName = products.SupplierName,
                CategoryName = products.CategoryName
            });
        }
        SubmitQuoteAsync(m);
        m.product = productList;
        return View(m);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
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
                return View();
            }

            var result = await signInManager.PasswordSignInAsync(m.LoginEmail, m.LoginPassword, m.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(m.LoginEmail);
                var userId = user.Id;
                HttpContext.Session.SetString("userid", userId);

                return RedirectToAction("Index");
            }
            //ModelState.AddModelError("", "Username or Password is incorrect");
            TempData["AlertFail"] = "Username or Password is incorrect";
            return View();
        }
        TempData["AlertFail"] = "Invalid Login Attempt. Try again";
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
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


                await userManager.AddToRoleAsync(user, "User");
                //await signInManager.SignInAsync(user, false); // Remove to restrict login before user comfirm the email
                return RedirectToAction("Index");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

        }
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            TempData["AlertFail"] = "Invalid trial";
            return RedirectToAction("Index");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["AlertFail"] = "Invalid trial";
            return RedirectToAction("Index");
        }

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            TempData["AlertSuccess"] = "Email verification is successful. You can login now";
            return RedirectToAction("Index");
        }
        TempData["AlertFail"] = "Fail to verify your email";
        return RedirectToAction("Index");
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

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SubmitQuoteAsync(Main m)
    {
        var getProduct = from products in db.products select products;

        var userId = userManager.GetUserId(User);
        var user = await userManager.FindByIdAsync(userId);

        Quote? quote = new Quote();

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
        RetrieveProducts(getProduct, quote);
        m.quote = quote;
        return View(m);
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

                await emailSender.SendEmailAsync(
                    "", //Put ToEmail address here
                    "[ELinkTech] User submitted a quote",
                    "User Information: " + quote.UserName + "(" + quote.UserEmail + ")<br>Quote about: " + productName + "<br>Message: " + quote.Message);

                TempData["AlertSuccess"] = "Your quote is successfully submitted";
            }
            catch (Exception e)
            {
                TempData["AlertFail"] = "Fail to submit your quote";
            }
        }
        return RedirectToAction("Index");
    }
}
