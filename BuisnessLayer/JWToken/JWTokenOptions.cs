using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BuisnessLayer.JWToken
{
    public class JWTokenOptions
    {
        public const string ISSUER = "AuthServer"; // token issuer (producer)
        public const string AUDIENCE = "AuthClient"; // token consumer
        internal const string KEY = "secretKeyQWERTY123";   // sequrity key
        public const int LIFETIME = 100; // token lifetime = 100 minutes
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
