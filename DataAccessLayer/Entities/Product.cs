using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace DataAccessLayer.Entities
{
    [Index("Name")]
    [Index("PlatformId")]
    [Index("DateCreated")]
    [Index("TotalRating")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PlatformId { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? TotalRating { get; set; }
        public int? GenreId { get; set; }
        public int? AgeRating { get; set; }
        public string? LogoLink { get; set; }
        public string? BackgroundLink { get; set; }
        public double? Price { get; set; }

        public int? Count { get; set; }
    }
}
