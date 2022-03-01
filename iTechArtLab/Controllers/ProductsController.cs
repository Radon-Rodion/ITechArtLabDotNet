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
using iTechArtLab.ActionFilters;
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
        public IActionResult GetTopPlatforms()
        {
            var topPlatforms = _productsManager.GetTop3Platforms(_context.Platforms);
            return View("TopPlatforms", topPlatforms.ToList());
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
        public IActionResult SearchGames(string term="", int limit=10, int offset=0)
        {
            try
            {
                var products = _productsManager.SearchByName(_context.Products, term ?? "", limit, offset);
                return View("Search",products.ToList());
            } catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return Redirect("/Home/Error");
            }
        }


        [HttpGet("list")]
        [ServiceFilter(typeof(SearchParamsFilterAsync))]
        public IActionResult ListGames(string nameFilter, int genreFilter, int ageFilter, string sortField, string? ascSorting, int elementsOnPage)
        {
            if (nameFilter == null) nameFilter = "";
            bool ascSort = ascSorting != null;
            var paginatedProducts = _productsManager.FilterSearchAndPaginate(nameFilter, genreFilter, ageFilter, sortField, ascSort, _context, elementsOnPage);
            return View("List", paginatedProducts);
        }

        /// <summary>
        /// Get request for product(game) information by its id
        /// </summary>
        /// <remarks>/api/games/id/{id}</remarks>
        /// <param name="id" example="3">Id of the game</param>
        /// <response code="200">View with Product(Game) full information</response>
        /// <response code="404">No such game found</response>
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetInfo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null || product.DateCreated == null)
            {
                return NotFound();
            }
            var model = new ProductViewModel(product);

            return View("Product",model);
        }

        /// <summary>
        /// Get request for createGameInfo form
        /// </summary>
        /// <remarks>/api/games</remarks>
        /// <response code="200">View with CreateGameInfo form</response>
        /// <response code="401">Sign in required</response>
        /// <response code="403">Admin role required</response>
        [HttpGet]
        public object GetCreateGameForm()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;
            return View("Create");
        }

        /// <summary>
        /// Post request to create new gameInfo
        /// </summary>
        /// <remarks>/api/games</remarks>
        /// <response code="201">New game info successfully created</response>
        /// <response code="401">Sign in required</response>
        /// <response code="403">Admin role required</response>
        [HttpPost]
        public async Task<object> CreateGameInfo(ProductViewModel model)
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            if (ModelState.IsValid)
            {
                Product newProduct = await _productsManager.AddNewProductAsync(_context, model, _cloudinaryManager);
                HttpContext.Response.StatusCode = 201;
                return Json(newProduct);
            }
            return View("Create", model);
        }

        /// <summary>
        /// Put request for game info update
        /// </summary>
        /// <remarks>/api/games</remarks>
        /// <param name="id" example="9">Game id</param>
        /// <param name="productName" example="Game1">new game name</param>
        /// <param name="platformId" example="2">new platform id</param>
        /// <param name="totalRating" example="90">new total rating</param>
        /// <param name="genreId" example="1">new genre id</param>
        /// <param name="ageRating" example="12">new age rating</param>
        /// <param name="logoLink" example="p:\photo.png">new logo link (local or web)</param>
        /// <param name="backgroundLink" example="http://someLink.com">new background link (local or web)</param>
        /// <param name="price" example="12.5">new price</param>
        /// <param name="count" example="23">new products amount</param>
        /// <response code="200">New(updated) game info</response>
        /// <response code="400">Errors during info validation</response>
        /// <response code="401">Sign in required</response>
        /// <response code="403">Admin role required</response>
        /// <response code="404">No such game found</response>
        [HttpPut]
        public async Task<object> UpdateGameInfo(int id, string productName, int platformId,
            int totalRating, int genreId, int ageRating, string logoLink, string backgroundLink, int price, int count)
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            var model = new ProductViewModel(id)
            {
                ProductName = productName,
                PlatformId = platformId,
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

        /// <summary>
        /// Delete request for product(game) information by its id
        /// </summary>
        /// <remarks>/api/games/id/{id}</remarks>
        /// <param name="id" example="3">Id of the game</param>
        /// <response code="204">Empty body</response>
        /// <response code="401">Sign in required</response>
        /// <response code="403">Admin role required</response>
        /// <response code="404">No such game found</response>
        [HttpDelete("id/{id}")]
        public async Task<object> DeleteGameInfo(int? id)
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager,
                _jWTokenValidator, Role.Name(Roles.Admin), _jWTokenConfig)) return errorResponse;

            if (id == null)
            {
                return NotFound("Didn't got id");
            }
            HttpContext.Response.StatusCode = 204;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                await _productsManager.DeleteSoftAsync(product, _context); //Soft deleting
            }
            
            return "{}";
        }

        [HttpGet("rating")]
        public async Task<object> GetRatingForm(int gameId)
        {
            string errorMessage;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) return errorMessage;

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == gameId);
            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            int userId = _sessionManager.GetUserId(HttpContext.Session);
            var rating = await _productsManager.FindRatingAsync(gameId, userId, _context);
            var model = new RatingViewModel(gameId);

            if (rating != null && !rating.IsDeleted)
                model.Rating = rating.Rating;

            return View("Rating", model);
        }

        [HttpPost("rating")]
        public async Task<object> RateGame(RatingViewModel model)
        {
            string errorMessage;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) return errorMessage;

            Log.Logger.Warning($"gameId: {model.GameId}; rating: {model.Rating}");
            if (!ModelState.IsValid)
            {
                return View("Rating", model);
            }

            int userId = _sessionManager.GetUserId(HttpContext.Session);
            var newRating = await _productsManager.RateProductAsync(model.GameId, userId, model.Rating, _context);

            var resultModel = new RatingViewModel(newRating.ProductId) { Rating = newRating.Rating };

            return View("Rating", resultModel);
        }

        [HttpDelete("rating")]
        public async Task<object> DeleteRating(int gameId)
        {
            string errorMessage;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) 
                return errorMessage;

            HttpContext.Response.StatusCode = 204;
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.ProductId == gameId);

            if (rating != null && !rating.IsDeleted)
                await _productsManager.DeleteRatingSoftAsync(rating, _context);
            return "{}";
        }
    }
}
