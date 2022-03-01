using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Data;

namespace TestProject.DataAcessLayerTests
{
    public class SortFieldsTest
    {
        [Fact]
        public void TestSearchFields()
        {
            Assert.Equal("Price", SortField.Name(SortFields.Price));
            Assert.Equal("Rating", SortField.Name(SortFields.Rating));
        }
    }
}
