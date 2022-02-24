using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Serilog;

namespace BuisnessLayer.JWToken
{
    public class JWTokenValidator
    {
        /// <summary>
        /// Validates token role
        /// </summary>
        /// <param name="authToken" example="AAbdfkle95k2ls+91DgsS67jd">token to be validated</param>
        /// <param name="role" example="Admin">role to be validated</param>
        /// <param name="config">JWT token configuration parameters</param>
        public static bool IsTokenRoleValid(string authToken, string role, JWTokenConfig config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(config);

            SecurityToken validatedToken;
            ClaimsPrincipal claims = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            //Log.Logger.Information($"{claims.}");

            if (!claims.IsInRole(role)) return false;
            return true;
        }

        private static TokenValidationParameters GetValidationParameters(JWTokenConfig config)
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ValidIssuer = config.Issuer,
                ValidAudience = config.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key)) // The same key as the one that generate the token
            };
        }
    }
}
