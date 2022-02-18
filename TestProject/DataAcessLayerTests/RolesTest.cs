using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Data;

namespace TestProject.DataAcessLayerTests
{
    public class RolesTest
    {
        [Fact]
        public void TestRoles()
        {
            Assert.Equal("Admin", Role.Name(Roles.Admin));
            Assert.Equal("User", Role.Name(Roles.User));
        }
    }
}
