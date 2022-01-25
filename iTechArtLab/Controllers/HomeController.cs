using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace iTechArtLab.Controllers
{
    [Route("/Home")]
    public class HomeController : Controller
    {
        [HttpGet("GetInfo")]
        public ObjectResult GetInfo()
        {
            Log.Logger.Information($"GetInfo requested at {DateTime.UtcNow.ToLongTimeString()}");
            //throw new Exception("Exception for Serilog in HomeController");
            return Ok("Hello world");
        }

        [HttpGet("Error")]
        public ObjectResult Error()
        {
            Log.Logger.Error($"An unhandled exception occured at {DateTime.UtcNow.ToLongTimeString()}");
            return Ok("Something went wrong...");
        }
    }
}
