using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Identity;
using System.Text;
using BuisnessLayer;
using BuisnessLayer.JWToken;

namespace iTechArtLab.Controllers
{
    [Route("/Home")]
    public class HomeController : Controller
    {

        [HttpGet("GetInfo")]
        public string GetInfo()
        {
            if (!SessionManager.HasToken(HttpContext.Session)) //check authenticated
            {
                HttpContext.Response.StatusCode = 401;
                return "Sign in first!";
            }
            if (!JWTokenValidator.ValidateTokenRole(SessionManager.GetToken(HttpContext.Session), "admin")) //check role
            {
                HttpContext.Response.StatusCode = 403;
                return "You are not admin!";
            }

            Log.Logger.Information($"GetInfo requested at {DateTime.UtcNow.ToLongTimeString()}");
            //throw new Exception("Exception for Serilog in HomeController");
            return "Hello world";
        }

        [HttpGet("Error1")]
        public string Error()
        {
            HttpContext.Response.StatusCode = 400;
            Log.Logger.Error($"An unhandled exception occured at {DateTime.UtcNow.ToLongTimeString()}");
            return "Something went wrong...";
        }
    }
}
