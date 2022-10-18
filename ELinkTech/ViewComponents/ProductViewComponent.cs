using Microsoft.AspNetCore.Mvc;
using ELinkTech.Models;

namespace ELinkTech.ViewComponents
{

    public class ProductViewComponent : ViewComponent
    {
        private DataContext _databaseContext;
        public ProductViewComponent(DataContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var product = from products in _databaseContext.products
                          join suppliers in _databaseContext.suppliers
                          on products.SupplierID equals suppliers.SupplierID
                          join categories in _databaseContext.categories
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
            return View(productList);
        }

    }
}
