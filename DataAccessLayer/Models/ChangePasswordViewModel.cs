using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// Password that is already set
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>123456_Password</example>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        /// <summary>
        /// New password to be set
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>111_NewPassword</example>
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$",
            ErrorMessage = "Required 1 uppercase, 1 lowercase, 1 non-alphanumeric and 1 numeric character. Length from 8 to 16")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        /// <summary>
        /// New password confirmation
        /// </summary>
        /// <remarks>Required, must be equal to NewPassword</remarks>
        /// <example>111_NewPassword</example>
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$",
            ErrorMessage = "Required 1 uppercase, 1 lowercase, 1 non-alphanumeric and 1 numeric character. Length from 8 to 16")]
        [Compare("NewPassword", ErrorMessage = "Passwords aren't equal")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string NewPasswordConfirm { get; set; }
    }
}
