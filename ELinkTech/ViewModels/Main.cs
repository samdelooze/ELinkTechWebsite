﻿using System.ComponentModel.DataAnnotations;

//********************************************************************
//Author(s): James
//Date: 14 / 11 / 2022
//Purpose: Main Model
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.ViewModels
{
    public class Main
    {
        public Models.Quote? quote { get; set; }
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
        //[Required] This affects to Login as well because all views are using one and the same model
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address*")]
        public string? RegisterEmail { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password*")]
        public string? RegisterPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password*")]
        [Compare(nameof(RegisterPassword), ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "First Name*")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name*")]
        public string? LastName { get; set; }

        [Display(Name = "Contact Number*")]
        public string? ContactNumber { get; set; }

        [Display(Name = "State")]
        public string? State { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Suburb")]
        public string? Suburb { get; set; }

        [Display(Name = "Postcode")]
        public string? Postcode { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
    }
}
