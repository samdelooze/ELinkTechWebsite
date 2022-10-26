using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELinkTech.Models
{
    public class Product
    {
        [Key]
        public string? ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }

        [Display(Name = "Supplier")]
        public string? SupplierID { get; set; }

        [Display(Name = "Category")]
        public string? CategoryID { get; set; }

        public List<SelectListItem>? SupplierList = new List<SelectListItem>();

        public List<SelectListItem>? CategoryList = new List<SelectListItem>();
    }
}
