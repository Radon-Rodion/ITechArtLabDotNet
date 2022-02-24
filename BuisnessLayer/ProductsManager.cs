using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using DataAccessLayer.Data;
using Serilog;

namespace BuisnessLayer
{
    public class ProductsManager
    {
        public static IEnumerable<Platform> Top3Platforms(IQueryable<Platform> platforms)//ApplicationDbContext context
        {
            return platforms.OrderByDescending(p => p.Products.Count).Take(3);
        }

        public static IEnumerable<Platform> Top3Platforms(IEnumerable<Platform> platforms)
        {
            return platforms.OrderByDescending(p => p.Products.Count).Take(3);
        }

        public static IEnumerable<Product> SearchByName(IEnumerable<Product> products, string namePart, int limit, int offset)
        {
            return products.Where(p => p.Name.ToLower().Contains(namePart.ToLower())).Skip(offset).Take(limit);
        }

        public static async Task AddNewProductAsync(ApplicationDbContext context, ProductViewModel model)
        {
            Product product = new Product()
            {
                Name = model.ProductName,
                PlatformId = System.Convert.ToInt32(model.PlatformId),
                DateCreated = DateTime.Now,
                TotalRating = System.Convert.ToInt32(model.TotalRating),
                GenreId = System.Convert.ToInt32(model.GenreId),
                AgeRating = System.Convert.ToInt32(model.AgeRating),
                LogoLink = model.LogoLink,
                BackgroundLink = model.BackgroundLink,
                Price = System.Convert.ToInt32(model.Price),
                Count = System.Convert.ToInt32(model.Count)
            };
            Log.Logger.Error($"Name: {model.ProductName} Id:{product.Id}");

            context.Add(product);
            await context.SaveChangesAsync();
        }
    }
}
