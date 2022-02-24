using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class ProductViewModel
    {
        /// <summary>
        /// Name of the product(game)
        /// </summary>
        /// <remarks>Required</remarks>
        /// <example>Monolite 5</example>
        [Required]
        [Display(Name = "Product name")]
        public string ProductName { get; set; }

        /// <summary>
        /// Id of the product(game) platform
        /// </summary>
        /// <remarks>Required, numeric</remarks>
        /// <example>1</example>
        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Must be numeric")]
        [Display(Name = "PlatformId")]
        public string PlatformId { get; set; }

        /// <summary>
        /// Total rating (mark) of the product(game)
        /// </summary>
        /// <remarks>Not required, numeric</remarks>
        /// <example>10</example>
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Must be numeric")]
        [Display(Name = "TotalRating")]
        public string TotalRating { get; set; }

        /// <summary>
        /// Id of the product(game) genre
        /// </summary>
        /// <remarks>Required, numeric</remarks>
        /// <example>3</example>
        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Must be numeric")]
        [Display(Name = "GenreId")]
        public string GenreId { get; set; }

        /// <summary>
        /// Age rating of the product(game)
        /// </summary>
        /// <remarks>Not required, numeric</remarks>
        /// <example>12</example>
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Must be numeric")]
        [Display(Name = "AgeRating")]
        public string AgeRating { get; set; }

        /// <summary>
        /// Link to the product(game) logo
        /// </summary>
        /// <remarks>Not required</remarks>
        /// <example>https://link.com</example>
        [Display(Name = "LogoLink")]
        public string LogoLink { get; set; }

        /// <summary>
        /// Link to the product(game) background
        /// </summary>
        /// <remarks>Not required</remarks>
        /// <example>https://link.com</example>
        [Display(Name = "BackgroundLink")]
        public string BackgroundLink { get; set; }

        /// <summary>
        /// Price of the product(game) in $
        /// </summary>
        /// <remarks>Not required, numeric</remarks>
        /// <example>20.5</example>
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Must be numeric")]
        [Display(Name = "Price")]
        public string Price { get; set; }

        /// <summary>
        /// Count of the products(games)
        /// </summary>
        /// <remarks>Not required, numeric</remarks>
        /// <example>23</example>
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Must be numeric")]
        [Display(Name = "Count")]
        public string Count { get; set; }
    }
}
