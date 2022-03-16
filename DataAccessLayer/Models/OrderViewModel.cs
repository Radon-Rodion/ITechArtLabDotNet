using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class OrderViewModel
    {
        [Required]
        [Display(Name = "Order")]
        public int OrderId { get; set; }

        [Display(Name = "Game name")]
        public string GameName { get; set; }

        [Display(Name = "Adding date")]
        public DateTime AddingDate { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public int Amount { get; set; }
    }
}
