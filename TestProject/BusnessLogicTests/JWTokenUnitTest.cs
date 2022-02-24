using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BuisnessLayer.JWToken;
using TestProject.Moq;

namespace TestProject.BuisnessLogicTests
{
    public class JWTokenUnitTest
    {
        const string MOQ_USER_NAME = "UserName";
        const string MOQ_USER_ROLE = "UserRole";

        [Fact]
        public void TestTokenConfig()
        {
            Assert.NotNull(MoqConfigs.jwtConfig.Issuer);
            Assert.NotNull(MoqConfigs.jwtConfig.Audience);
            Assert.True(MoqConfigs.jwtConfig.Lifetime > 0, "Token lifetime is negative or 0");
            Assert.NotNull(MoqConfigs.jwtConfig.GetSymmetricSecurityKey());
        }

        [Fact]
        public void TestTokenGeneratorNegtive()
        {
            Assert.Throws<ArgumentNullException>(() => JWTokenGenerator.GenerateToken(MOQ_USER_NAME, null, MoqConfigs.jwtConfig));
            Assert.Throws<ArgumentNullException>(() => JWTokenGenerator.GenerateToken(null, MOQ_USER_ROLE, MoqConfigs.jwtConfig));
        }

        [Fact]
        public void TestTokenGeneratorPositive()
        {
            var token1 = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE, MoqConfigs.jwtConfig);
            var token2 = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE, MoqConfigs.jwtConfig);
            Assert.True(token1.Substring(0, 50) == token2.Substring(0, 50), "Tokens with same data beginings must be equal!");
        }

        [Fact]
        public void TestTokenValidatorNegative()
        {
            var invalidToken = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, "", MoqConfigs.jwtConfig);

            Assert.False(JWTokenValidator.IsTokenRoleValid(invalidToken, MOQ_USER_ROLE, MoqConfigs.jwtConfig), "Invalid token validation fail");
        }

        [Fact]
        public void TestTokenValidatorPositive()
        {
            var validToken = JWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE, MoqConfigs.jwtConfig);

            Assert.True(JWTokenValidator.IsTokenRoleValid(validToken, MOQ_USER_ROLE, MoqConfigs.jwtConfig), "Valid token validation fail");
        }
    }
}
