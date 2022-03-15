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
            return platforms.AsParallel().OrderByDescending(p => p.Products.Where(prod => !prod.IsDeleted).Count()).Take(3);
        }

        public IEnumerable<Platform> GetTop3Platforms(IEnumerable<Platform> platforms)
        {
            return platforms.AsParallel().OrderByDescending(p => p.Products.Where(prod => !prod.IsDeleted).Count()).Take(3);
        }

        public IEnumerable<Product> SearchByName(IEnumerable<Product> products, string namePart, int limit, int offset)
        {
            return products.Where(p => !p.IsDeleted && p.Name.ToLower().Contains(namePart.ToLower())).Skip(offset).Take(limit);
        }

        public async Task<List<Product>> FilterSearchAndPaginateAsync(string nameFilter, int genreFilter, int ageFilter, 
            string sortField, bool ascSorting, ApplicationDbContext context, int elementsOnPage, int pageNumber)
        {
            int sortParam = ascSorting ? 1 : -1;

            var productsFitting = context.Products.Where(p => !p.IsDeleted && p.Name.ToLower().Contains(nameFilter.ToLower().Trim())
                && (genreFilter == -1 || p.GenreId == genreFilter) && (ageFilter == -1 || p.AgeRating == ageFilter))
                .OrderBy(p => sortField == SortField.Name(SortFields.Price) ? p.Price * sortParam : p.TotalRating * sortParam)
                .Skip((pageNumber-1)*elementsOnPage).Take(elementsOnPage).AsNoTracking();
            return await productsFitting.ToListAsync();
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
                Count = System.Convert.ToInt32(model.Count),
                IsDeleted = false
            };

            context.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductFromModelAsync(Product product, ProductViewModel model, ApplicationDbContext context, CloudinaryManager cloudinaryManager)
        {
            product.Name = model.ProductName;
            product.PlatformId = model.PlatformId;
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
            product.IsDeleted = true;

            context.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Product product, ApplicationDbContext context)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        private async Task RecalculateTotalRatingAsync(int productId, ApplicationDbContext context)
        {
            long ratingsSum = 0;
            var ratingsOfProduct = context.Ratings.Where(r => r.ProductId == productId && !r.IsDeleted);
            Parallel.ForEach(ratingsOfProduct, rating =>
            {
                ratingsSum += rating.Rating;
            });
                

            var product = await context.Products.FindAsync(productId);
            if (ratingsOfProduct.Count() != 0)
                product.TotalRating = Convert.ToInt32(ratingsSum / ratingsOfProduct.Count());
            else product.TotalRating = 0;

            context.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task<ProductRating> FindRatingAsync(int productId, int userId, ApplicationDbContext context)
        {
            var rating = context.Ratings.Where(r => r.ProductId == productId && r.UserId == userId).AsNoTracking().FirstOrDefault();
            return rating;
        }

        public async Task<ProductRating> RateProductAsync(int productId, int userId, int ratingValue, ApplicationDbContext context)
        {
            var rating = await FindRatingAsync(productId, userId, context);

            if(rating == null) //doesn't rating of this user about this product already exists(ever existed)?
            {
                rating = new ProductRating() { ProductId = productId, UserId = userId, Rating = ratingValue, IsDeleted = false };
                context.Add(rating);
            } else
            {
                rating.Rating = ratingValue;
                rating.IsDeleted = false;
                context.Update(rating);
            }
            await context.SaveChangesAsync();
            await RecalculateTotalRatingAsync(productId, context);
            return rating;
        }

        public async Task DeleteRatingHardAsync(ProductRating rating, ApplicationDbContext context)
        {
            context.Ratings.Remove(rating);
            await context.SaveChangesAsync();
            await RecalculateTotalRatingAsync(rating.ProductId, context);
        }

        public async Task DeleteRatingSoftAsync(ProductRating rating, ApplicationDbContext context)
        {
            rating.IsDeleted = true;
            await context.SaveChangesAsync();
            await RecalculateTotalRatingAsync(rating.ProductId, context);
        }
    }
}
