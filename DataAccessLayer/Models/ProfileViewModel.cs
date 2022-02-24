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
        /// <summary>
        /// UserName of the profile
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>User008</example>
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        /// <summary>
        /// Email of the profile
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>email01example@tut.by</example>
        [Required]
        [RegularExpression(@"^.+@([\w]+)(\.(\w){2,3})$", ErrorMessage = "Only emails are allowed")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Delivery address string of the profile
        /// </summary>
        /// <remarks>Not required</remarks>
        /// <example>Minsk, Cheluskintsev, 18/21</example>
        [Display(Name = "Delivery")]
        public string Delivery { get; set; }

        /// <summary>
        /// Phone number string of the profile
        /// </summary>
        /// <remarks>not required</remarks>
        /// <example>+375251892399</example>
        [RegularExpression(@"^\+(?=.*\d){10,12}$",
            ErrorMessage = "Only phone numbers started with '+' and with length from 10 to 12 numbers are allowed")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
