using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//********************************************************************
//Author(s): James
//Date: 14 / 11 / 2022
//Purpose: Product Model
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Models
{
    public class Product
    {
        [Key]
        public int? ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierID { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }

        [Display(Name = "Image")]
        public string? ProductImage { get; set; }

        [Display(Name = "Details")]
        public string? ProductDetails { get; set; }

        [NotMapped]
        [Display(Name = "Supplier Name")]
        public string? SupplierName { get; set; }

        [NotMapped]
        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }

        [NotMapped]
        [Display(Name = "Select a Supplier")]
        public List<SelectListItem>? SupplierList = new List<SelectListItem>();

        [NotMapped]
        [Display(Name = "Select a Category")]
        public List<SelectListItem>? CategoryList = new List<SelectListItem>();
    }
}
