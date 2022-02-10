using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BuisnessLayer
{
    public class SessionManager
    {
        private const string KEY_FOR_JWTOKEN = "AccessToken";
        public static void SetToken(ISession session, string jwtToken)
        {
            session.Set(KEY_FOR_JWTOKEN, Encoding.ASCII.GetBytes(jwtToken));
        }

        public static string GetToken(ISession session)
        {
            return Encoding.ASCII.GetString(getTokenInBytes(session));
        }

        public static bool HasToken(ISession session)
        {
            return getTokenInBytes(session) != null;
        }

        private static byte[] getTokenInBytes(ISession session)
        {
            byte[] arr;
            session.TryGetValue(KEY_FOR_JWTOKEN, out arr);
            return arr;
        }
    }
}
