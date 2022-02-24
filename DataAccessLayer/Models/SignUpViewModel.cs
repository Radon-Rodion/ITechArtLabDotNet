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
        /// <summary>
        /// Email of new account
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>email01example@tut.by</example>
        [Required]
        [RegularExpression(@"^.+@([\w]+)(\.(\w){2,3})$", ErrorMessage = "Only emails are allowed")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Password for new account
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>_12345Password</example>
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", 
            ErrorMessage = "Required 1 uppercase, 1 lowercase, 1 non-alphanumeric and 1 numeric character. Length from 8 to 16")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Password confirmation for new account
        /// </summary>
        /// <remarks>Required, must be equal to Password</remarks>
        /// <example>_12345Password</example>
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
