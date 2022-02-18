using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Text;
using BuisnessLayer;
using DataAccessLayer.Data;
using BuisnessLayer.JWToken;

namespace iTechArtLab.Controllers
{
    [Route("/Home")]
    public class HomeController : Controller
    {
        private readonly JWTokenConfig _jWTokenConfig;

        public HomeController(IOptions<JWTokenConfig> jWTokenOptions)
        {
            _jWTokenConfig = jWTokenOptions.Value;
        }

        [HttpGet("GetInfo")]
        public string GetInfo()
        {
            string errorResponse;
            if (!AccessControlManager.ValidateAccess(HttpContext, out errorResponse, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            Log.Logger.Information($"GetInfo requested at {DateTime.UtcNow.ToLongTimeString()}");
            HttpContext.Response.StatusCode = 200;
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
