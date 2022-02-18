using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^.+@([\w]+)(\.(\w){2,3})$", ErrorMessage = "Only emails are allowed")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Delivery")]
        public string Delivery { get; set; }

        [RegularExpression(@"^\+(?=.*\d){10,12}$",
            ErrorMessage = "Only phone numbers started with '+' and with length from 10 to 12 numbers are allowed")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
