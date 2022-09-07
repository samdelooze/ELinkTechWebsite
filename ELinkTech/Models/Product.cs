using System.ComponentModel.DataAnnotations;

namespace ELinkTech.Models
{
    public class Product
    {
        [Key]
        public string ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
    }
}
