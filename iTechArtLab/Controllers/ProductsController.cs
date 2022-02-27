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
using BuisnessLayer.Cloudinary;
using BuisnessLayer.JWToken;
using Microsoft.Extensions.Options;
using Serilog;

namespace iTechArtLab.Controllers
{
    [Route("/api/games")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductsManager _productsManager;
        private readonly ModelValidator _modelValidator;
        private readonly CloudinaryManager _cloudinaryManager;
        private readonly JWTokenConfig _jWTokenConfig;
        private readonly AccessControlManager _accessControlManager;
        private readonly SessionManager _sessionManager;
        private readonly JWTokenValidator _jWTokenValidator;

        public ProductsController(ApplicationDbContext context, SessionManager sessionManager, ProductsManager productsManager,
            JWTokenValidator jWTokenValidator, AccessControlManager accessControlManager, ModelValidator modelValidator,
            IOptions<JWTokenConfig> jWTokenOptions, IOptions<CloudinaryConfig> cloudinaryOptions)
        {
            _context = context;
            _jWTokenConfig = jWTokenOptions.Value;

            _productsManager = productsManager;
            _modelValidator = modelValidator;
            _cloudinaryManager = new CloudinaryManager
                (cloudinaryOptions.Value.ApiKey, cloudinaryOptions.Value.ApiSecret, cloudinaryOptions.Value.CloudName);

            _sessionManager = sessionManager;
            _jWTokenValidator = jWTokenValidator;
            _accessControlManager = accessControlManager;
        }

        /// <summary>
        /// Get request for 3 top platorms
        /// </summary>
        /// <remarks>/api/games/top-platforms</remarks>
        /// <response code="200">View with Top-3 platforms</response>
        [HttpGet("top-platforms")]
        public IActionResult TopPlatforms()
        {
            var topPlatforms = _productsManager.GetTop3Platforms(_context.Platforms);
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
                var products = _productsManager.SearchByName(_context.Products, term ?? "", limit, offset);
                return View(products.ToList());
            } catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetInfo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null || product.DateCreated == null)
            {
                return NotFound();
            }
            var model = new ProductViewModel(product);

            return View("Product",model);
        }

        [HttpGet]
        public object Create()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;
            return View();
        }

        [HttpPost]
        public async Task<object> Create(ProductViewModel model)
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            if (ModelState.IsValid)
            {
                await _productsManager.AddNewProductAsync(_context, model, _cloudinaryManager);
                return RedirectToAction(nameof(Search));
            }
            return View(model);
        }

        [HttpPut]
        public async Task<object> Update(int id, string productName, int platformId,
            int totalRating, int genreId, int ageRating, string logoLink, string backgroundLink, int price, int count)
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel(id)
            {
                ProductName = productName,
                PlatformId = platformId,
                TotalRating = totalRating,
                GenreId = genreId,
                AgeRating = ageRating,
                LogoLink = logoLink,
                BackgroundLink = backgroundLink,
                Price = price,
                Count = count
            };

            var modelErrors = _modelValidator.ValidateViewModel(model);

            if (modelErrors.Count == 0)
            {
                await _productsManager.UpdateProductFromModelAsync(product, model, _context, _cloudinaryManager);
                return Json(product);
            }

            HttpContext.Response.StatusCode = 400;
            return _modelValidator.StringifyErrors(modelErrors);
        }

        [HttpDelete("id/{id}")]
        public async Task<object> Delete(int? id)
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            if (id == null)
            {
                return NotFound("Didn't got id");
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound($"Product with id {id} not found");
            }
            await _productsManager.DeleteSoftAsync(product, _context);
            HttpContext.Response.StatusCode = 204;
            return "{}";
        }
    }
}
