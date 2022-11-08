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

        public async Task<IActionResult> Index()
        {
            GetSuppliers();
            return View();
        }
        //Quotes
        [HttpGet]
        public IActionResult GetQuotes()
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

            foreach (var quotes in quote)
            {
                quotesList.Add(new Quote
                {
                    QuoteId = quotes.QuoteId,
                    ProductID = quotes.ProductID,
                    UserID = quotes.UserID,
                    UserEmail = quotes.UserEmail,
                    Message = quotes.Message,
                });
            }
            return View(quotesList);
        }

        //Categories
        [HttpGet]
        public IActionResult GetCategories()
        {

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

            return View(categoryList);
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

            return View(supplierList);
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
            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteSupplier(int id)
        {
            Supplier supplier = db.suppliers.Find(id);
            db.suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
