using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ELinkTech.ViewModels
{
    public class Email
    {
        [Display(Name = "To")]
        public string? toEmail { get; set; }

        [Display(Name = "From")]
        public string? fromEmail { get; set; }

        [Display(Name = "Subject")]
        public string? subject { get; set; }

        [Display(Name = "Message")]
        public string? message { get; set; }
    }
}

