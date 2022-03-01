using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class RatingViewModel
    {
        /// <summary>
        /// Id of the rated product(game)
        /// </summary>
        /// <remarks>Not required, because must not be changed</remarks>
        /// <example>5</example>
        [Display(Name = "Game")]
        public int GameId { get; set; }

        /// <summary>
        /// Total rating (mark) of the product(game)
        /// </summary>
        /// <remarks>Required, numeric</remarks>
        /// <example>10</example>
        [Required]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        public RatingViewModel(int gameId)
        {
            GameId = gameId;
        }
        public RatingViewModel() {}
    }
}
