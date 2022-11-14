using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ELinkTech.Models;
using Microsoft.AspNetCore.Identity;
using ELinkTech.ViewModels;
using Microsoft.Win32;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

//********************************************************************
//Author(s): Sam, Soyeong
//Date: 14 / 11 / 2022
//Purpose: Dealing with Main Index Page, Register, Login and Logout
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Controllers;

public class MainController : Controller

{
    private UserManager<ApplicationUser> userManager { get; }
    private SignInManager<ApplicationUser> signInManager { get; }
    private RoleManager<IdentityRole> roleManager { get; }
    private readonly DataContext db;

    private IEmailSender emailSender;
    private readonly IConfiguration configuration;

    private readonly ILogger _logger;

    
    public MainController(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          RoleManager<IdentityRole> roleManager,
                          DataContext db,
                          IEmailSender emailSender,
                          IConfiguration configuration,
                          ILogger<MainController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.db = db;
        this.emailSender = emailSender;
        this.configuration = configuration;
        this._logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Main m)
    {
        try
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

            m.product = productList;
            return View(m);

        }
        catch (Exception e)
        {
            string sError = "class: MainController, function: Index\n" + e.Message + "\n";
            sError += e.StackTrace;
            _logger.LogError(sError);
        }
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
        try
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
        catch (Exception e)
        {
            string sError = "class: MainController, function: Login-Post\n" + e.Message + "\n";
            sError += e.StackTrace;
            _logger.LogError(sError);
        }

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
        try
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
        catch (Exception e)
        {
            string sError = "class: MainController, function: Register-Post\n" + e.Message + "\n";
            sError += e.StackTrace;
            _logger.LogError(sError);
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
        try
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
        catch (Exception e)
        {
            string sError = "class: MainController, function: ConfirmEmail\n" + e.Message + "\n";
            sError += e.StackTrace;
            _logger.LogError(sError);

            TempData["AlertFail"] = "Fail to verify your email";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> LogoutYes()//sign out user
    {
        try
        {
            await signInManager.SignOutAsync();
            TempData["AlertSuccess"] = "You are successfully logged out";
            return RedirectToAction("Index");

        }
        catch (Exception e)
        {
            string sError = "class: MainController, function: LogoutYes\n" + e.Message + "\n";
            sError += e.StackTrace;
            _logger.LogError(sError);

            TempData["AlertFail"] = "Fail to logout";
        }

        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public void RetrieveProducts(IQueryable<Product> p, Quote q)
    {
        try
        {
            foreach (var product in p)
            {
                SelectListItem item = new SelectListItem();
                item.Text = product.ProductName;
                item.Value = product.ProductID.ToString();
                q.ProductList.Add(item);
            }
        }
        catch (Exception e)
        {
            string sError = "class: MainController, function: RetrieveProducts\n" + e.Message + "\n";
            sError += e.StackTrace;
            _logger.LogError(sError);
        }
    }
}
