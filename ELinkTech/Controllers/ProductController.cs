using ELinkTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Entity;

namespace ELinkTech.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext db;

        public ProductController(DataContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            var supplier = from suppliers in db.suppliers select suppliers;

            var category = from categories in db.categories select categories;
            Product product = new Product();

            RetrieveSuppliers(supplier, product);
            RetrieveCategories(category, product);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            /*var product = new Product()
            {
                ProductID = newProduct.ProductID,
                ProductName = newProduct.ProductName,
            };*/

            await db.products.AddAsync(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Main");
        }

        private void RetrieveSuppliers(IQueryable<Supplier> s, Product p)
        {
            foreach (var supplier in s)
            {
                SelectListItem item = new SelectListItem();
                item.Text = supplier.SupplierName;
                item.Value = supplier.SupplierID;
                p.SupplierList.Add(item);
            }
        }

        private void RetrieveCategories(IQueryable<Category> c, Product p)
        {
            foreach(var category in c)
            {
                SelectListItem item = new SelectListItem();
                item.Text = category.CategoryName;
                item.Value = category.CategoryID.ToString();
                p.CategoryList.Add(item);
            }
        }

        [HttpGet]
        public IActionResult FindProducts()
        {
            var product = from products in db.products
                    join suppliers in db.suppliers
                    on products.SupplierID equals suppliers.SupplierID
                    join categories in db.categories
                    on products.CategoryID equals categories.CategoryID
                    select new
                    {
                        ProductName = products.ProductName,
                        SupplierName = suppliers.SupplierName,
                        CategoryName = categories.CategoryName
                    };

            List<Product> productList = new List<Product>();

            foreach(var products in product)
            {
                productList.Add(new Product
                {
                    ProductName = products.ProductName,
                    SupplierID = products.SupplierName,
                    CategoryID = products.CategoryName
                });
            }

            return View(productList);
        }

        public void FindSingleProduct(Product product)
        {

        }

        [HttpGet]
        public IActionResult UpdateProduct(string id)
        {
            Product product = db.products.Find(id);


            //Product product = db.products.Where(p => p.ProductID == id).FirstOrDefault();

           // var supplier = from suppliers in db.suppliers select suppliers;

            //var category = from categories in db.categories select categories;

            //RetrieveSuppliers(supplier, product);
            //RetrieveCategories(category, product);
            return View(product);
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product p)
        {
            Product product = db.products.Find(p);
            db.products.Update(product);
            return View(product);
        }

        [HttpGet]
        public IActionResult DeleteProduct()
        {
            return View();
        }
    }
}
