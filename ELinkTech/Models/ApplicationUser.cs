using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ELinkTech.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? State { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? Postcode { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
