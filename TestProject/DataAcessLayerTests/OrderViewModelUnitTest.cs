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
    public class OrderViewModelUnitTest
    {
        ModelValidator modelValidator = new ModelValidator();
        [Fact]
        public void OrderViewModelPositiveTest()
        {
            var validModel = new OrderViewModel()
            {
                OrderId = 1,
                Amount = 2
            };
            var errorcount = modelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void OrderViewModelNegativeTest()
        {
            var invalidModel = new OrderViewModel()
            {
                GameName = "Name1",
                AddingDate = DateTime.Now
            };
            var errorcount = modelValidator.ValidateViewModel(invalidModel).Count();
            Assert.Equal(0, errorcount);
            Assert.Equal(0, invalidModel.OrderId);
            Assert.Equal(0, invalidModel.Amount);
        }
    }
}
