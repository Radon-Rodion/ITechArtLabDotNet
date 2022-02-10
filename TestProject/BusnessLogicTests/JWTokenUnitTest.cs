using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BuisnessLayer.JWToken;

namespace TestProject.BuisnessLogicTests
{
    public class JWTokenUnitTest
    {
        const string MOQ_USER_NAME = "UserName";
        const string MOQ_USER_ROLE = "UserRole";

        [Fact]
        public void TestTokenOptions()
        {
            Assert.NotNull(JWTokenOptions.ISSUER);
            Assert.NotNull(JWTokenOptions.AUDIENCE);
            Assert.True(JWTokenOptions.LIFETIME > 0, "Token lifetime is negative or 0");
            Assert.NotNull(JWTokenOptions.GetSymmetricSecurityKey());
        }

        [Fact]
        public void TestTokenGenerator()
        {
            Assert.Throws<ArgumentNullException>(() => JWTokenGenerator.GenerateToken(MOQ_USER_NAME, null));
            Assert.Throws<ArgumentNullException>(() => JWTokenGenerator.GenerateToken(null, MOQ_USER_ROLE));

            var token1 = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE);
            var token2 = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE);
            Assert.True(token1.Substring(0, 50) == token2.Substring(0, 50), "Tokens with same data beginings must be equal!");
        }

        [Fact]
        public void TestTokenValidator()
        {
            var validToken = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE);
            var invalidToken = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, "");

            Assert.True(JWTokenValidator.ValidateTokenRole(validToken, MOQ_USER_ROLE), "Valid token validation fail");
            Assert.False(JWTokenValidator.ValidateTokenRole(invalidToken, MOQ_USER_ROLE), "Invalid token validation fail");
        }
    }
}
