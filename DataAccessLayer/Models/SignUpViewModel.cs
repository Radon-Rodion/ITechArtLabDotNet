using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class SignUpViewModel
    {
        [Required]
        [RegularExpression(@"^.+@([\w]+)(\.(\w){2,3})$", ErrorMessage = "Only emails are allowed")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", 
            ErrorMessage = "Required 1 uppercase, 1 lowercase, 1 non-alphanumeric and 1 numeric character. Length from 8 to 16")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", 
            ErrorMessage = "Required 1 uppercase, 1 lowercase, 1 non-alphanumeric and 1 numeric character. Length from 8 to 16")]
        [Compare("Password", ErrorMessage = "Passwords aren't equal")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string PasswordConfirm { get; set; }

        public override string ToString()
        {
            return $"Email: {Email}; Password: {Password}; PasswordConfirm: {PasswordConfirm} ";
        }
    }
}
