using ELinkTech.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ELinkTech.ViewModels;
using ELinkTech.Services;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;

//********************************************************************
//Author(s): Sam, Soyeong
//Date: 14 / 11 / 2022
//Purpose: Managing Quotes, Categories, and Suppliers
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly DataContext db;
      
        private IEmailSender emailSender;
        private readonly IConfiguration configuration;

        private readonly ILogger _logger;

        public AdminController(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               DataContext db,
                               IEmailSender emailSender,
                               IConfiguration configuration,
                               ILogger<AdminController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this._logger = logger;
        }

        public async Task<IActionResult> AdminForm()
        {
            return PartialView("AdminForm");
        }

        //Quotes
        [HttpGet]
        public async Task<IActionResult> GetQuotesAsync()
        {
            try
            {
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

                if (quote == null) return View(quotesList);
                
                foreach (var quotes in quote)
                {
                    if(quotes != null)
                    {
                        var user = await userManager.FindByIdAsync(quotes.UserID);
                        if (user == null) TempData["AlertFail"] = "Fail to find user ID from database";

                        var product = db.products.Where(m => m.ProductID.ToString() == quotes.ProductID.ToString()).FirstOrDefault();
                        if (product == null) TempData["AlertFail"] = "Fail to find service ID from database";

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
                }
                return View(quotesList);
                
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: GetQuotes\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult Reply(int quoteId)
        {
            try
            {
                var quote = db.quotes.Where(m => m.QuoteId == quoteId).FirstOrDefault();
                var product = db.products.Where(m => m.ProductID.ToString() == quote.ProductID.ToString()).FirstOrDefault();

                if (quote != null && product != null)
                {
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
                        subject = "[ELinkTech]Answer to your quote request on " + product.ProductName + "(\"" + shortQuoteMessage + "\")",
                        message = message
                    };

                    return PartialView("Reply", email);
                }

                TempData["AlertFail"] = "Fail to find quote ID or service ID from database";

            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: Reply-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetQuotes");
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
                    string sError = "class: AdminController, function: Reply-Post\n" + e.Message + "\n";
                    sError += e.StackTrace;
                    _logger.LogError(sError);

                    TempData["AlertFail"] = "Fail to send your reply";
                }
            }

            return RedirectToAction("GetQuotes");
        }

        //Categories
        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                var category = from categories in db.categories
                               select new
                               {
                                   CategoryID = categories.CategoryID,
                                   CategoryName = categories.CategoryName,
                               };
                List<Category> categoryList = new List<Category>();

                if (category == null) return View(categoryList);
           
                foreach (var categories in category)
                {
                    if (categories != null)
                    {
                        categoryList.Add(new Category
                        {
                            CategoryID = categories.CategoryID,
                            CategoryName = categories.CategoryName,
                        });
                    }
                }
                return View(categoryList);

            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: GetCategories\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            try
            {
                Category category = new Category();
                return View(category);
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: AddCategory-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetCategories");
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(Category category)
        {
            try
            {
                await db.categories.AddAsync(category);
                await db.SaveChangesAsync();
                return RedirectToAction("GetCategories");
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: AddCategory-Post\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetCategories");
        }

        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            try
            {
                Category category = db.categories.Where(s => s.CategoryID == id).FirstOrDefault();
                return View(category);
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: UpdateCategory-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetCategories");
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            try
            {
                db.categories.Update(category);
                db.SaveChanges();
                return RedirectToAction("GetCategories");
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: UpdateCategory-Post\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetCategories");
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                Category category = db.categories.Find(id);
                db.categories.Remove(category);
                db.SaveChanges();
                return RedirectToAction("GetCategories");
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: DeleteCategory\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetCategories");
        }

        //Suppliers
        [HttpGet]
        public IActionResult GetSuppliers()
        {
            try
            {
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

                if (supplier == null) return View(supplierList);

                foreach (var suppliers in supplier)
                {
                    if(suppliers != null)
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
                }
                return View(supplierList);

            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: GetCategories\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("Index", "Main");
        }

        [HttpGet]
        public IActionResult AddSupplier()
        {
            try
            {
                Supplier supplier = new Supplier();
                return View(supplier);
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: AddSupplier-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetSuppliers");
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            try
            {
                await db.suppliers.AddAsync(supplier);
                await db.SaveChangesAsync();
                return RedirectToAction("GetSuppliers");
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: AddSupplier-Post\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetSuppliers");
        }

        [HttpGet]
        public IActionResult UpdateSupplier(int id)
        {
            try
            {
                Supplier supplier = db.suppliers.Where(s => s.SupplierID == id).FirstOrDefault();
                return View(supplier);
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: UpdateSupplier-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetSuppliers");
        }

        [HttpPost]
        public async Task <IActionResult> UpdateSupplier(Supplier s)
        {
            try
            {
                db.suppliers.Update(s);
                db.SaveChanges();
                return RedirectToAction("GetSuppliers");
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: UpdateSupplier-Post\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetSuppliers");
        }

        [HttpGet]
        public IActionResult DeleteSupplier(int id)
        {
            try
            {
                Supplier supplier = db.suppliers.Find(id);
                db.suppliers.Remove(supplier);
                db.SaveChanges();
                return RedirectToAction("GetSuppliers");
            }
            catch (Exception e)
            {
                string sError = "class: AdminController, function: DeleteSupplier\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("GetSuppliers");
        }
    }
}
