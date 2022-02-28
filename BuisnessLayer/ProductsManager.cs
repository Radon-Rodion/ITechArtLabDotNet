using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using DataAccessLayer.Data;
using BuisnessLayer.Cloudinary;
using Serilog;

namespace BuisnessLayer
{
    public class ProductsManager
    {
        public IEnumerable<Platform> GetTop3Platforms(IQueryable<Platform> platforms)//ApplicationDbContext context
        {
            return platforms.OrderByDescending(p => p.Products.Where(prod => prod.DateCreated != null).Count()).Take(3);
        }

        public IEnumerable<Platform> GetTop3Platforms(IEnumerable<Platform> platforms)
        {
            return platforms.OrderByDescending(p => p.Products.Where(prod => prod.DateCreated != null).Count()).Take(3);
        }

        public IEnumerable<Product> SearchByName(IEnumerable<Product> products, string namePart, int limit, int offset)
        {
            return products.Where(p => p.DateCreated != null && p.Name.ToLower().Contains(namePart.ToLower())).Skip(offset).Take(limit);
        }

        public async Task<Product> AddNewProductAsync(ApplicationDbContext context, ProductViewModel model, CloudinaryManager cloudinaryManager)
        {
            Product product = new Product()
            {
                Name = model.ProductName,
                PlatformId = System.Convert.ToInt32(model.PlatformId),
                DateCreated = DateTime.Now,
                TotalRating = System.Convert.ToInt32(model.TotalRating),
                GenreId = System.Convert.ToInt32(model.GenreId),
                AgeRating = System.Convert.ToInt32(model.AgeRating),
                LogoLink = cloudinaryManager.UploadImage(model.LogoLink),
                BackgroundLink = cloudinaryManager.UploadImage(model.BackgroundLink),
                Price = System.Convert.ToInt32(model.Price),
                Count = System.Convert.ToInt32(model.Count)
            };

            context.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductFromModelAsync(Product product, ProductViewModel model, ApplicationDbContext context, CloudinaryManager cloudinaryManager)
        {
            product.Name = model.ProductName;
            product.PlatformId = model.PlatformId;
            product.TotalRating = model.TotalRating;
            product.GenreId = model.GenreId;
            product.AgeRating = model.AgeRating;
            product.LogoLink = cloudinaryManager.UploadImage(model.LogoLink);
            product.BackgroundLink = cloudinaryManager.UploadImage(model.BackgroundLink);
            product.Price = model.Price;
            product.Count = model.Count;

            context.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Product product, ApplicationDbContext context)
        {
            product.DateCreated = null;
            product.PlatformId = null;
            product.GenreId = null;

            context.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Product product, ApplicationDbContext context)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
