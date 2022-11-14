using System.ComponentModel.DataAnnotations;

//********************************************************************
//Author(s): James
//Date: 14 / 11 / 2022
//Purpose: Supplier Model
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Contact")]
        public string Phone { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "Suburb")]
        public string Suburb { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Postcode")]
        public string Postcode { get; set; }
    }
}
