using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BuisnessLayer.JWToken;
namespace BuisnessLayer
{
    public class AccessControlManager
    {
        public static bool IsTokenValid(HttpContext context, out string response, string role = null, JWTokenConfig tokenConfig = null)
        {
            response = null;
            if (!SessionManager.HasToken(context.Session)) //check authenticated
            {
                context.Response.StatusCode = 401;
                response = "Sign in first!";
            }
            if (role!= null && !JWTokenValidator.IsTokenRoleValid(SessionManager.GetToken(context.Session), role, tokenConfig)) //check role
            {
                context.Response.StatusCode = 403;
                response = "You are not admin!";
            }
            return response == null;
        }
    }
}
