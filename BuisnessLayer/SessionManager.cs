using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BuisnessLayer
{
    public class SessionManager
    {
        private const string KEY_FOR_JWTOKEN = "AccessToken";
        private const string KEY_FOR_USER_ID = "UserId";
        public static void SetToken(ISession session, string jwtToken)
        {
            session.Set(KEY_FOR_JWTOKEN, Encoding.ASCII.GetBytes(jwtToken));
        }

        public static string GetToken(ISession session)
        {
            return Encoding.ASCII.GetString(GetValueInBytes(session, KEY_FOR_JWTOKEN));
        }

        public static bool HasToken(ISession session)
        {
            return GetValueInBytes(session, KEY_FOR_JWTOKEN) != null;
        }

        public static void SetUserId(ISession session, int userId)
        {
            session.Set(KEY_FOR_USER_ID, BitConverter.GetBytes(userId));
        }

        public static int GetUserId(ISession session)
        {
            var id = BitConverter.ToInt32(GetValueInBytes(session, KEY_FOR_USER_ID));
            Log.Logger.Information($"id: {id}");
            return id;
        }

        private static byte[] GetValueInBytes(ISession session, string key)
        {
            byte[] arr;
            session.TryGetValue(key, out arr);
            session.TryGetValue(key, out arr);
            return arr;
        }
    }
}
