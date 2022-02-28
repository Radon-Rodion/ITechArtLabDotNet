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
        private readonly AccessControlManager _accessControlManager;
        private readonly SessionManager _sessionManager;
        private readonly JWTokenValidator _jWTokenValidator;

        public HomeController(IOptions<JWTokenConfig> jWTokenOptions, 
            SessionManager sessionManager, JWTokenValidator jWTokenValidator, AccessControlManager accessControlManager)
        {
            _jWTokenConfig = jWTokenOptions.Value;
            _sessionManager = sessionManager;
            _jWTokenValidator = jWTokenValidator;
            _accessControlManager = accessControlManager;
        }

        /// <summary>
        /// Test method returning HelloWorld and available for admins only
        /// </summary>
        /// <remarks>/Home/GetInfo</remarks>
        /// <response code="200">Hello world</response>
        /// <response code="401">Sign in required</response>
        /// <response code="403">Admin role required</response>
        [HttpGet("GetInfo")]
        public string GetInfo()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager, 
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

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
