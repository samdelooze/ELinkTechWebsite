using ELinkTech.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ELinkTech.ViewModels;
using ELinkTech.Services;
using Newtonsoft.Json.Linq;

//*******************************************************************
//Author(s): Sam, Soyeong
//Date: 14 / 11 / 2022
//Perpose:
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly DataContext db;

        private IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public AdminController(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               DataContext db,
                               IEmailSender emailSender,
                               IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
            this.emailSender = emailSender;
            this.configuration = configuration;
        }

        public async Task<IActionResult> AdminForm()
        {
            return PartialView("AdminForm");
        }
        //Quotes
        [HttpGet]
        public async Task<IActionResult> GetQuotesAsync()
        {
            if (ModelState.IsValid)
            {
                try {
                var quote = from quotes in db.quotes
                            select new
                            {
                                QuoteId = quotes.QuoteId,
                                ProductID = quotes.ProductID,
                                UserID = quotes.UserID,
                                UserEmail = quotes.UserEmail,
                                Message = quotes.Message,
                            };
                List<Quote> quotesList = new List<Quote>();
                    if (quotesList.Count >= 1)
                    {
                        foreach (var quotes in quote)
                        {
                            var user = await userManager.FindByIdAsync(quotes.UserID);

                            var product = db.products.Where(m => m.ProductID.ToString() == quotes.ProductID).FirstOrDefault();
                            var productName = product.ProductName;

                            quotesList.Add(new Quote
                            {
                                QuoteId = quotes.QuoteId,
                                ProductID = quotes.ProductID,
                                ProductName = productName,
                                UserID = quotes.UserID,
                                UserName = user.FirstName + " " + user.LastName,
                                UserEmail = quotes.UserEmail,
                                Message = quotes.Message,
                            });
                        }
                        return View(quotesList);
                    }
                }
                catch
                {
                    TempData["AlertFail"] = "No catagories exist yet";
                }
            }
            return RedirectToAction("Index","Main");
        }

        [HttpGet]
        public IActionResult Reply(int quoteId)
        {
            var quote = db.quotes.Where(m => m.QuoteId == quoteId).FirstOrDefault();
            var product = db.products.Where(m => m.ProductID.ToString() == quote.ProductID).FirstOrDefault();

            if (string.IsNullOrEmpty(quote.Message)) quote.Message = "";
            string shortQuoteMessage = quote.Message.Length <= 20 ? quote.Message : quote.Message.Substring(0, 20) + "...";

            string message = "";
            message += "<br><br> -----------------------------------------------------------------------------";
            message += "<br> Quote on: " + product.ProductName;
            message += "<br> Message: " + quote.Message;

            Email email = new Email
            {
                toEmail = quote.UserEmail,
                fromEmail = configuration["SendGrid:SenderEmail"],
                subject = "[ELinkTech]Answer to your quote on " + product.ProductName + "(\"" + shortQuoteMessage + "\")",
                message = message
            };
            return PartialView("Reply", email);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(Email email)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await emailSender.SendEmailAsync(email.toEmail, email.subject, email.message);

                    TempData["AlertSuccess"] = "Reply is successfully sent to " + email.toEmail;
                }
                catch (Exception e)
                {
                    TempData["AlertFail"] = "Fail to send your reply";
                }
            }
            return RedirectToAction("GetQuotes");
        }

        //Categories
        [HttpGet]
        public IActionResult GetCategories()
        {
            if (ModelState.IsValid)
            {
                try {
                var category = from categories in db.categories
                               select new
                               {
                                   CategoryID = categories.CategoryID,
                                   CategoryName = categories.CategoryName,
                               };
                List<Category> categoryList = new List<Category>();
           
                        foreach (var categories in category)
                        {
                            categoryList.Add(new Category
                            {
                                CategoryID = categories.CategoryID,
                                CategoryName = categories.CategoryName,
                            });
                        }
                    if (categoryList.Count >= 1)
                    {
                        return View(categoryList);
                    }
            }
                catch
                {
                    TempData["AlertFail"] = "No categories exist yet";
                }
            }
            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            Category category = new Category();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(Category category)
        {
            await db.categories.AddAsync(category);
            await db.SaveChangesAsync();
            return RedirectToAction("GetCategories");
        }

        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            Category category = db.categories.Where(s => s.CategoryID == id).FirstOrDefault();
            return View(category);
        }
        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            db.categories.Update(category);
            db.SaveChanges();
            return RedirectToAction("GetCategories");
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            Category category = db.categories.Find(id);
            db.categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("GetCategories");
        }

        //Suppliers
        [HttpGet]
        public IActionResult GetSuppliers()
        {
            if (ModelState.IsValid)
            {
                try {
                    var supplier = from suppliers in db.suppliers
                                   select new
                                   {
                                       SupplierID = suppliers.SupplierID,
                                       SupplierName = suppliers.SupplierName,
                                       Phone = suppliers.Phone,
                                       Email = suppliers.Email,
                                       Street = suppliers.Street,
                                       Suburb = suppliers.Suburb,
                                       State = suppliers.State,
                                       Postcode = suppliers.Postcode,
                                   };
                    List<Supplier> supplierList = new List<Supplier>();
                    
                        foreach (var suppliers in supplier)
                        {
                            supplierList.Add(new Supplier
                            {
                                SupplierID = suppliers.SupplierID,
                                SupplierName = suppliers.SupplierName,
                                Phone = suppliers.Phone,
                                Email = suppliers.Email,
                                Street = suppliers.Street,
                                Suburb = suppliers.Suburb,
                                State = suppliers.State,
                                Postcode = suppliers.Postcode
                            });
                        }
                    if (supplierList.Count >= 1)
                    {
                        return View(supplierList);
                    }
                }
                catch
                {
                    TempData["AlertFail"] = "No suppliers exist yet";
                }
    }
            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult AddSupplier()
        {
            Supplier supplier = new Supplier();
            
            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            await db.suppliers.AddAsync(supplier);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult UpdateSupplier(int id)        {
            Supplier supplier = db.suppliers.Where(s => s.SupplierID == id).FirstOrDefault();
            return View(supplier);
        }

        [HttpPost]
        public async Task <IActionResult> UpdateSupplier(Supplier s)
        {
            db.suppliers.Update(s);
            db.SaveChanges();
            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult DeleteSupplier(int id)
        {
            Supplier supplier = db.suppliers.Find(id);
            db.suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index", "Main");
        }

    }
}
