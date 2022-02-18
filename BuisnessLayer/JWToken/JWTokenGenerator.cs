using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BuisnessLayer.JWToken
{
    public class JWTokenGenerator
    {
        private static ClaimsIdentity GetClaimsIdentity(string userName, string userRole)
        {
            if (userName == null || userRole == null) throw new ArgumentNullException("userName or/and userRole");

            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        public static string GenerateToken(string userName, string userRole, JWTokenConfig config)
        {
            var identity = GetClaimsIdentity(userName, userRole);

            var now = DateTime.UtcNow;
            // creating token
            var jwt = new JwtSecurityToken(
                    issuer: config.Issuer,
                    audience: config.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(config.Lifetime)),
                    signingCredentials: new SigningCredentials(config.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
