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
        JWTokenGenerator jWTokenGenerator = new JWTokenGenerator();
        JWTokenValidator jWTokenValidator = new JWTokenValidator();

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
            Assert.Throws<ArgumentNullException>(() => jWTokenGenerator.GenerateToken(MOQ_USER_NAME, null, MoqConfigs.jwtConfig));
            Assert.Throws<ArgumentNullException>(() => jWTokenGenerator.GenerateToken(null, MOQ_USER_ROLE, MoqConfigs.jwtConfig));
        }

        [Fact]
        public void TestTokenGeneratorPositive()
        {
            var token1 = jWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE, MoqConfigs.jwtConfig);
            var token2 = jWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE, MoqConfigs.jwtConfig);
            Assert.True(token1.Substring(0, 50) == token2.Substring(0, 50), "Tokens with same data beginings must be equal!");
        }

        [Fact]
        public void TestTokenValidatorNegative()
        {
            var invalidToken = jWTokenGenerator.GenerateToken(MOQ_USER_NAME, "", MoqConfigs.jwtConfig);

            Assert.False(jWTokenValidator.IsTokenRoleValid(invalidToken, MOQ_USER_ROLE, MoqConfigs.jwtConfig), "Invalid token validation fail");
        }

        [Fact]
        public void TestTokenValidatorPositive()
        {
            var validToken = jWTokenGenerator.GenerateToken(MOQ_USER_NAME, MOQ_USER_ROLE, MoqConfigs.jwtConfig);

            Assert.True(jWTokenValidator.IsTokenRoleValid(validToken, MOQ_USER_ROLE, MoqConfigs.jwtConfig), "Valid token validation fail");
        }
    }
}
