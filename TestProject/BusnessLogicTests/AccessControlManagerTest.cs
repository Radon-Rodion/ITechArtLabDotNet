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
        JWTokenGenerator jWTokenGenerator = new JWTokenGenerator();
        AccessControlManager accessControlManager = new AccessControlManager();
        SessionManager sessionManager = new SessionManager();
        JWTokenValidator jWTokenValidator = new JWTokenValidator();

        [Fact]
        public void TestAccessValidateNegative()
        {
            Assert.Throws<NullReferenceException>(() => accessControlManager.IsTokenValid(null, out responseText, sessionManager));
            Assert.False(accessControlManager.IsTokenValid(context, out responseText, sessionManager));
            Assert.Equal("Sign in first!", responseText);
            Assert.Equal(401, context.Response.StatusCode);
        }

        [Fact]
        public void TestAccessValidatePartialAccess()
        {
            var token = jWTokenGenerator.GenerateToken("SomeName", "SomeRole", MoqConfigs.jwtConfig);
            sessionManager.SetToken(context.Session, token);

            Assert.True(accessControlManager.IsTokenValid(context, out responseText, sessionManager));
            Assert.Null(responseText);

            Assert.False(accessControlManager.IsTokenValid(context, out responseText, sessionManager, jWTokenValidator, ROLE, MoqConfigs.jwtConfig));
            Assert.Equal("You are not admin!", responseText);
            Assert.Equal(403, context.Response.StatusCode);
        }

        [Fact]
        public void TestAccessValidateFullAccess()
        {
            var token = jWTokenGenerator.GenerateToken("SomeName", ROLE, MoqConfigs.jwtConfig);
            sessionManager.SetToken(context.Session, token);

            Assert.True(accessControlManager.IsTokenValid(context, out responseText, sessionManager));
            Assert.Null(responseText);

            Assert.True(accessControlManager.IsTokenValid(context, out responseText, sessionManager, jWTokenValidator, ROLE, MoqConfigs.jwtConfig));
            Assert.Null(responseText);
        }
    }
}
