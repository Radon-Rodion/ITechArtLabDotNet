using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entities
{
    public class User : IdentityUser<int>
    {
        public string Delivery { get; set; }
        public ICollection<ProductRating> Ratings { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
