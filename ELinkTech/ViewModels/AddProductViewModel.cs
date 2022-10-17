using System.ComponentModel.DataAnnotations;

namespace ELinkTech.ViewModels
{
    public class AddProductViewModel
    {
        [Key]
        public string ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
}
