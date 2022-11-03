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

        //Categories
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
        public IActionResult UpdateSupplier(int id)
        {
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
