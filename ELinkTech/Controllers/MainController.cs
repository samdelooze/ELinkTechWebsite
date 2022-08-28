using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ELinkTech.Models;
using Microsoft.AspNetCore.Identity;
using ELinkTech.ViewModels;
using Microsoft.Win32;

namespace ELinkTech.Controllers;

public class MainController : Controller

{
    private UserManager<ApplicationUser> userManager { get; }
    private SignInManager<ApplicationUser> signInManager { get; }

    private readonly DataContext db;
    
    public MainController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DataContext db)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.db = db;
    }
    [HttpGet]
    public IActionResult Index()
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
            var result = await signInManager.PasswordSignInAsync(m.LoginEmail, m.LoginPassword, m.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Username or Password is incorrect");
        }
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Register(Main m)//create an account
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { 
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
                //await userManager.AddToRoleAsync(user, "User");
                await signInManager.SignInAsync(user, false);
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
