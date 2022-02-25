using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer;
using DataAccessLayer.Models;

namespace TestProject.DataAcessLayerTests
{
    public class ProfileViewModelsUnitTest
    {
        [Fact]
        public void SignInViewModelPositiveTest()
        {
            var validModel = new ProductViewModel()
            {
                ProductName = "someName",
                PlatformId = 1,
                GenreId = 2
            };
            var errorcount = ModelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void SignInViewModelNegativeTest()
        {
            var modelWithout2Fields = new ProductViewModel()
            {
                ProductName = "SomeName2",
                TotalRating = 10,
                AgeRating = 12,
                LogoLink = "SomeLink1",
                BackgroundLink = "SomeLink2",
                Price = 21.3,
                Count = 19
            };
            var errorcount = ModelValidator.ValidateViewModel(modelWithout2Fields).Count();
            Assert.Equal(2, errorcount);
        }
    }
}
