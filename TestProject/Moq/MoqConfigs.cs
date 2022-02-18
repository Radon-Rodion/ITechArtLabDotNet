using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer.JWToken;
using BuisnessLayer.Senders;

namespace TestProject.Moq
{
    class MoqConfigs
    {
        public static JWTokenConfig jwtConfig = new JWTokenConfig() 
            { Issuer = "AuthServer",  Audience = "AuthClient", Key = "secretKeyQWERTY123", Lifetime = 100};

        public static SmtpConfig smtpConfig = new SmtpConfig()
        { HostName = "smtp.gmail.com", Port = 587, UserName = "radon0rodion@gmail.com", UserPassword = "8334191test" };
    }
}
