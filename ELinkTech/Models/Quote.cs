﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELinkTech.Models
{
    public class Quote
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? QuoteId { get; set; }

        [Required]
        [Display(Name = "Service ID")]
        public string? ProductID { get; set; }

        [NotMapped]
        [Display(Name = "Service Name")]
        public string? ProductName { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public string? UserID { get; set; }

        [NotMapped]
        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Required]
        [Display(Name = "User Email")]
        public string? UserEmail { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string? Message { get; set; }
        
        public List<SelectListItem>? ProductList = new List<SelectListItem>();

    }
}