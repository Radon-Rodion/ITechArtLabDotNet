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
        public string Issuer { get; set; } // token issuer (producer) AuthServer
        public string Audience { get; set; } // token consumer AuthClient
        public string Key { get; set; }   // sequrity key secretKeyQWERTY123
        public int Lifetime { get; set; } // token lifetime = n minutes 100
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
