using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer;
using Microsoft.AspNetCore.Http;
using TestProject.Moq;
using BuisnessLayer.JWToken;

namespace TestProject.BusnessLogicTests
{
    public class AccessControlManagerTest
    {
        HttpContext context = new MoqHttpContext();
        string responseText = "";
        const string ROLE = "Admin";

        [Fact]
        public void TestAccessValidateNegative()
        {
            Assert.Throws<NullReferenceException>(() => AccessControlManager.IsTokenValid(null, out responseText));
            Assert.False(AccessControlManager.IsTokenValid(context, out responseText));
            Assert.Equal("Sign in first!", responseText);
            Assert.Equal(401, context.Response.StatusCode);
        }

        [Fact]
        public void TestAccessValidatePartialAccess()
        {
            var token = JWTokenGenerator.GenerateToken("SomeName", "SomeRole", MoqConfigs.jwtConfig);
            SessionManager.SetToken(context.Session, token);

            Assert.True(AccessControlManager.IsTokenValid(context, out responseText));
            Assert.Null(responseText);

            Assert.False(AccessControlManager.IsTokenValid(context, out responseText, ROLE, MoqConfigs.jwtConfig));
            Assert.Equal("You are not admin!", responseText);
            Assert.Equal(403, context.Response.StatusCode);
        }

        [Fact]
        public void TestAccessValidateFullAccess()
        {
            var token = JWTokenGenerator.GenerateToken("SomeName", ROLE, MoqConfigs.jwtConfig);
            SessionManager.SetToken(context.Session, token);

            Assert.True(AccessControlManager.IsTokenValid(context, out responseText));
            Assert.Null(responseText);

            Assert.True(AccessControlManager.IsTokenValid(context, out responseText, ROLE, MoqConfigs.jwtConfig));
            Assert.Null(responseText);
        }
    }
}
