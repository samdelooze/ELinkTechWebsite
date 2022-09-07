using System.ComponentModel.DataAnnotations;
namespace ELinkTech.Models
{
    public class Order
    {
        [Key]

        public string OrderId { get; set; }

        [Display(Name = "Quantity")]
        public string Quantity { get; set; }

        [Display(Name = "Total Price")]
        public float TotalPrice { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
    }
}
