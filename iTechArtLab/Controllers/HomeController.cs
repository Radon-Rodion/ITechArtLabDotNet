﻿using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace iTechArtLab.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public ObjectResult GetInfo()
        {
            return Ok("Hello world");
        }
    }
}
