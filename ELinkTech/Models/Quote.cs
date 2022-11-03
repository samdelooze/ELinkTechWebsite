using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELinkTech.Models
{
    public class Quote
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? QuoteId { get; set; }
        public string? ProductID { get; set; }
        public string? UserID { get; set; }
        public string? UserEmail { get; set; }
        public string? Message { get; set; }

        public List<SelectListItem>? ProductList = new List<SelectListItem>();

    }
}