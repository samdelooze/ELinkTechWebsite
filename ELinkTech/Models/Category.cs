using System.ComponentModel.DataAnnotations;

//********************************************************************
//Author(s): James
//Date: 14 / 11 / 2022
//Purpose: Category Model
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
    }
}
