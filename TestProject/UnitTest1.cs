using System;
using Xunit;
using BuisnessLayer;

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public void TestMultiplication()
        {
            Assert.Equal(2 * 2, Class1.Multiply(2,2));
        }
    }
}
