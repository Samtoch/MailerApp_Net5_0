using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MailerApp_Net5_0.Models
{
    public class Email
    {
        [Required(ErrorMessage = "Your Email Address is Required")]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Email eg. john@gmail.com")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid Email Format.")]
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachement { get; set; }

    }
}
