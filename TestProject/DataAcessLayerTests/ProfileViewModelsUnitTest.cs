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
        ModelValidator modelValidator = new ModelValidator();
        [Fact]
        public void ProductViewModelPositiveTest()
        {
            var validModel = new ProductViewModel()
            {
                ProductName = "someName",
                PlatformId = 1,
                GenreId = 2
            };
            var errorcount = modelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void ProductViewModelNegativeTest()
        {
            var modelWithout2Fields = new ProductViewModel()
            {
                TotalRating = 10,
                AgeRating = 12,
                LogoLink = "SomeLink1",
                BackgroundLink = "SomeLink2",
                Price = 21.3,
                Count = 19
            };
            var errorcount = modelValidator.ValidateViewModel(modelWithout2Fields).Count();
            Assert.Equal(1, errorcount);
        }
    }
}
