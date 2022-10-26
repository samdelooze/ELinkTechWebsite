using System.ComponentModel.DataAnnotations;

namespace ELinkTech.ViewModels
{
    public class Main
    {
        public List<Models.Product>? product { get; set; }

        //LOGIN
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string? LoginEmail { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? LoginPassword { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }

        //REGISTER
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string? RegisterEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? RegisterPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(RegisterPassword), ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [Required]
        [Display(Name = "State")]
        public string? State { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "Suburb")]
        public string? Suburb { get; set; }

        [Required]
        [Display(Name = "Postcode")]
        public string? Postcode { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
    }
}
