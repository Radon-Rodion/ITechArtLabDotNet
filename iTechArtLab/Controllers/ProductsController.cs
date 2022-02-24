using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using BuisnessLayer;
using Serilog;

namespace iTechArtLab.Controllers
{
    [Route("/api/games")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get request for 3 top platorms
        /// </summary>
        /// <remarks>/api/games/top-platforms</remarks>
        /// <response code="200">View with Top-3 platforms</response>
        [HttpGet("top-platforms")]
        public IActionResult TopPlatforms()
        {
            var topPlatforms = ProductsManager.Top3Platforms(_context.Platforms);
            return View(topPlatforms.ToList());
        }

        /// <summary>
        /// Get request for products(games) list containing entered term
        /// </summary>
        /// <remarks>/api/games/search</remarks>
        /// <param name="term" example="Mon">products name filter</param>
        /// <param name="limit" example="5">amount of products to be shown (may be less)</param>
        /// <param name="offset" example="3">amount of products from start to be skipped</param>
        /// <response code="200">View with Products(Games)</response>
        [HttpGet("search")]
        public IActionResult Search(string term="", int limit=10, int offset=0)
        {
            try
            {
                var products = ProductsManager.SearchByName(_context.Products, term ?? "", limit, offset);
                return View(products.ToList());
            } catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet("id/")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                await ProductsManager.AddNewProductAsync(_context, model);
                return RedirectToAction(nameof(Search));
            }
            return View(model);
        }
    }
}
