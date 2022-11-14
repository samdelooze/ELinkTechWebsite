using ELinkTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Entity;

//********************************************************************
//Author(s): James, Soyeong
//Date: 14 / 11 / 2022
//Purpose: Managing Products
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProductController : Controller
    {
        private readonly DataContext db;
        private readonly ILogger _logger;

        public ProductController(DataContext db, ILogger<ProductController> logger)
        {
            this.db = db;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            try
            {
                var supplier = from suppliers in db.suppliers select suppliers;
                var category = from categories in db.categories select categories;
                Product product = new Product();

                RetrieveSuppliers(supplier, product);
                RetrieveCategories(category, product);
                return View(product);

            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: AddProduct-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("FindProducts");
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            try
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
            catch (Exception e)
            {
                string sError = "class: ProductController, function: AddProduct-Post\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("FindProducts");
        }

        private void RetrieveSuppliers(IQueryable<Supplier> s, Product p)
        {
            try
            {
                foreach (var supplier in s)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = supplier.SupplierName;
                    item.Value = supplier.SupplierID.ToString();
                    p.SupplierList.Add(item);
                }
            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: RetrieveSuppliers\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }
        }

        private void RetrieveCategories(IQueryable<Category> c, Product p)
        {
            try
            {
                foreach (var category in c)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = category.CategoryName;
                    item.Value = category.CategoryID.ToString();
                    p.CategoryList.Add(item);
                }
            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: RetrieveCategories\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }
        }

        [HttpGet]
        public IActionResult FindProducts()
        {
            try
            {
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

                if(product == null) return View(productList);

                foreach (var products in product)
                {
                    if(products != null)
                    {
                        productList.Add(new Product
                        {
                            ProductID=products.ProductID,
                            ProductName = products.ProductName,
                            ProductImage=products.ProductImage,
                            ProductDetails = products.ProductDetails,
                            SupplierName = products.SupplierName,
                            CategoryName = products.CategoryName
                        });
                    }
                }
                return View(productList);
            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: FindProducts\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("Index", "Main");
        }

        public void FindSingleProduct(Product product)
        {

        }

        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            try
            {
                var supplier = from suppliers in db.suppliers select suppliers;
                var category = from categories in db.categories select categories;
                Product? product = db.products.Where(p => p.ProductID == id).FirstOrDefault();
            
                RetrieveSuppliers(supplier, product);
                RetrieveCategories(category, product);
                return View(product);
            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: UpdateProduct-Get\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("FindProducts");
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product p)
        {
            try
            {
                //Product? product = db.products.Find(p.ProductID);
                db.Entry(p).State = (Microsoft.EntityFrameworkCore.EntityState.Modified);
                //db.products.Update(p);
                db.SaveChanges();
                return RedirectToAction("Index", "Main");
            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: UpdateProduct-Post\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("FindProducts");
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                Product product = db.products.Find(id);
                db.products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("FindProducts");
            }
            catch (Exception e)
            {
                string sError = "class: ProductController, function: DeleteProduct\n" + e.Message + "\n";
                sError += e.StackTrace;
                _logger.LogError(sError);
            }

            return RedirectToAction("FindProducts");
        }
    }
}
