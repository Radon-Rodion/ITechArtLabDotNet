using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public ICollection<Product> Products { get; set; }
        public Genre()
        {
            Products = new List<Product>();
        }
    }
}
