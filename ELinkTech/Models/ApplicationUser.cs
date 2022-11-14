using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

//********************************************************************
//Author(s): James
//Date: 14 / 11 / 2022
//Purpose:
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "State")]
        public string? State { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Suburb")]
        public string? Suburb { get; set; }

        [Display(Name = "Postcode")]
        public string? Postcode { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
    }
}
