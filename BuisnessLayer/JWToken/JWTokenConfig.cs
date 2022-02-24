using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BuisnessLayer.JWToken
{
    public class JWTokenConfig
    {
        /// <summary>
        /// Token issuer (producer)
        /// </summary>
        /// <example>AuthServer</example>
        public string Issuer { get; set; }

        /// <summary>
        /// Token consumer
        /// </summary>
        /// <example>AuthClient</example>
        public string Audience { get; set; }

        /// <summary>
        /// Sequrity key
        /// </summary>
        /// <example>secretKeyQWERTY123</example>
        public string Key { get; set; }

        /// <summary>
        /// token lifetime in minutes
        /// </summary>
        /// <example>100</example>
        public int Lifetime { get; set; }
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        }
    }
}
