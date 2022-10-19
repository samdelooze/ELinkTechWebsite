﻿using System.ComponentModel.DataAnnotations;
namespace ELinkTech.Models
{
    public class Category
    {
        [Key]
        public string CategoryID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
    }
}
