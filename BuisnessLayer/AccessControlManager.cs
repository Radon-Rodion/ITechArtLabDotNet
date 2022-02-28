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
        public bool IsTokenValid(HttpContext context, out string response, SessionManager sessionManager,
            JWTokenValidator validator = null, string role = null, JWTokenConfig tokenConfig = null)
        {
            response = null;
            if (!sessionManager.HasToken(context.Session)) //check authenticated
            {
                context.Response.StatusCode = 401;
                response = "Sign in first!";
                return false;
            }
            if (role!= null && !validator.IsTokenRoleValid(sessionManager.GetToken(context.Session), role, tokenConfig)) //check role
            {
                context.Response.StatusCode = 403;
                response = "You are not admin!";
                return false;
            }
            return true;
        }
    }
}
