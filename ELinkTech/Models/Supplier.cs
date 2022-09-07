using System.ComponentModel.DataAnnotations;

namespace ELinkTech.Models
{
    public class Supplier
    {
        [Key]
        public string SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Supplier Number")]
        public string Phone { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Street Address")]
        public string Street { get; set; }

        [Display(Name = "Suburb Address")]
        public string Suburb { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Postcode")]
        public string Postcode { get; set; }
    }
}
