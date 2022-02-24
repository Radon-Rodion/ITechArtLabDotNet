using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer;
using TestProject.Moq;
using DataAccessLayer.Entities;

namespace TestProject.BusnessLogicTests
{
    public class ProductsManagerUnitTest
    {
        List<Platform> platforms = new List<Platform>();
        List<Product> products = new List<Product>();

        [Fact]
        public void TestTopPlatformsNegative()
        {
            Assert.Throws<ArgumentNullException>(() => ProductsManager.Top3Platforms(null));
            var tempPlatforms = new List<Platform>();
            Assert.Empty(ProductsManager.Top3Platforms(tempPlatforms));
            tempPlatforms.Add(new Platform() { PlatformId = 1, PlatformName = "Platform" });
            Assert.Collection(ProductsManager.Top3Platforms(tempPlatforms), item =>
            {
                Assert.Equal(1, item.PlatformId);
                Assert.Equal("Platform", item.PlatformName);
            });
        }

        [Fact]
        public void TestTopPlatformsPositive()
        {
            if (platforms.Count == 0) FillListsWithMoqData();

            var topPlatforms = ProductsManager.Top3Platforms(platforms);
            Assert.Collection(topPlatforms, item =>
            {
                Assert.Equal(5, item.PlatformId);
            }, item=>
            {
                Assert.Equal(3, item.PlatformId);
            }, item =>
            {
                Assert.Equal(1, item.PlatformId);
            }
            );
        }

        [Fact]
        public void TestSearchNegative()
        {
            Assert.Throws<ArgumentNullException>(() => ProductsManager.SearchByName(null, "", 1, 0));

            var tempProducts = new List<Product>();

            Assert.Empty(ProductsManager.SearchByName(tempProducts, "", 1, 0));
            tempProducts.Add(new Product() { Id= 1, Name= "SomeName" });

            Assert.Empty(ProductsManager.SearchByName(tempProducts, "", 0, 0));
            Assert.Equal(1, ProductsManager.SearchByName(tempProducts, "", 10, 0).Count());
        }

        [Fact]
        public void TestSearchPositive()
        {
            if (platforms.Count == 0) FillListsWithMoqData();

            Assert.Collection(ProductsManager.SearchByName(products, "1", 3, 1), item =>
            {
                Assert.Equal("Game10", item.Name);
            }, item =>
            {
                Assert.Equal("Game11", item.Name);
            }, item =>
            {
                Assert.Equal("Game12", item.Name);
            });

            Assert.Collection(ProductsManager.SearchByName(products, "Game", 2,0), item =>
            {
                Assert.Equal("Game1", item.Name);
            }, item =>
            {
                Assert.Equal("Game2", item.Name);
            });
        }

        private void FillListsWithMoqData()
        {
            Platform newPlatform = new Platform() { PlatformId = 1, PlatformName = "Platform1" };
            Product newProduct = new Product() { Id = 1, Name = "Game1", PlatformId = 1 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 2, Name = "Game2", PlatformId = 1 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 3, Name = "Game3", PlatformId = 1 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);
            platforms.Add(newPlatform);


            newPlatform = new Platform() { PlatformId = 2, PlatformName = "Platform2" };
            newProduct = new Product() { Id = 4, Name = "Game4", PlatformId = 2 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 5, Name = "Game5", PlatformId = 2 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);
            platforms.Add(newPlatform);

            newPlatform = new Platform() { PlatformId = 3, PlatformName = "Platform3" };
            newProduct = new Product() { Id = 6, Name = "Game6", PlatformId = 3 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 7, Name = "Game7", PlatformId = 3 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 8, Name = "Game8", PlatformId = 3 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 9, Name = "Game9", PlatformId = 3 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);
            platforms.Add(newPlatform);

            platforms.Add(new Platform() { PlatformId = 4, PlatformName = "Platform4" });

            newPlatform = new Platform() { PlatformId = 5, PlatformName = "Platform5" };
            newProduct = new Product() { Id = 10, Name = "Game10", PlatformId = 5 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 11, Name = "Game11", PlatformId = 5 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 12, Name = "Game12", PlatformId = 5 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 13, Name = "Game13", PlatformId = 5 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);

            newProduct = new Product() { Id = 14, Name = "Game14", PlatformId = 5 };
            products.Add(newProduct);
            newPlatform.Products.Add(newProduct);
            platforms.Add(newPlatform);
        }
    }
}
