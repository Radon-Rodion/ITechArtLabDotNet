using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Entities;
using BuisnessLayer;
using BuisnessLayer.JWToken;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Serilog;

namespace iTechArtLab.Controllers
{
    [Route("/api/orders")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OrdersManager _ordersManager;
        private readonly ModelValidator _modelValidator;
        private readonly JWTokenConfig _jWTokenConfig;
        private readonly AccessControlManager _accessControlManager;
        private readonly SessionManager _sessionManager;
        private readonly JWTokenValidator _jWTokenValidator;

        public OrdersController(ApplicationDbContext context, SessionManager sessionManager, OrdersManager ordersManager,
            JWTokenValidator jWTokenValidator, AccessControlManager accessControlManager, ModelValidator modelValidator,
            IOptions<JWTokenConfig> jWTokenOptions)
        {
            _context = context;
            _jWTokenConfig = jWTokenOptions.Value;

            _ordersManager = ordersManager;
            _modelValidator = modelValidator;

            _sessionManager = sessionManager;
            _jWTokenValidator = jWTokenValidator;
            _accessControlManager = accessControlManager;
        }

        [HttpGet]
        public async Task<object> GetOrders()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager)) 
                return errorResponse;

            int userId = _sessionManager.GetUserId(HttpContext.Session);
            var orders = await _ordersManager.GetOrdersAsync(userId, _context);

            var models = _ordersManager.CreateViewModelsListFromOrders(orders, _context);
            return View("List", models);
        }

        [HttpPost]
        public async Task<object> CreateOrder(int productId, int amount)
        {
            try
            {
                string errorResponse;
                if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager))
                    return errorResponse;
                int userId = _sessionManager.GetUserId(HttpContext.Session);
                var order = await _ordersManager.FindOrderAsync(userId, productId, _context);
                if (order == null)
                    order = await _ordersManager.CreateOrderAsync(userId, productId, amount, _context);
                else
                    order = await _ordersManager.IncreaseOrderAmount(order, amount, _context);

                return _ordersManager.CreateViewModelFromOrder(order, _context);
            } catch(Exception e)
            {
                Log.Logger.Error(e.Message);
                Log.Logger.Error(e.StackTrace);
                return e;
            }
        }

        [HttpPut]
        public async Task<object> EditOrders()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager))
                return errorResponse;

            try
            {
                OrderViewModel[] orderInfos = await HttpContext.Request.ReadFromJsonAsync<OrderViewModel[]>();
                
                var orders = await _ordersManager.UpdateOrdersAsync(orderInfos, _context);
                var models = _ordersManager.CreateViewModelsListFromOrders(orders, _context);
                return View("List", models);
            } catch (JsonException e)
            {
                HttpContext.Response.StatusCode = 400;
                return "Invalid request content: required array of ids and amounts in JSON";
            }
            /*catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                Log.Logger.Error(e.StackTrace);
                return $"{e.Message}\n{e.StackTrace}";
            }*/
        }

        [HttpDelete]
        public async Task<object> DeleteOrders()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager))
                return errorResponse;
            try
            {
                int[] idsToDelete = await HttpContext.Request.ReadFromJsonAsync<int[]>();
                var orders = new List<Order>();
                foreach (int orderId in idsToDelete)
                {
                    var order = await _ordersManager.FindOrderAsync(orderId, _context);
                    if (order != null)
                        orders.Add(order);
                }
                await _ordersManager.DeleteMultipleSoftAsync(orders, _context);
                HttpContext.Response.StatusCode = 204;
                return "{}";
            } catch(JsonException e)
            {
                HttpContext.Response.StatusCode = 400;
                return "Invalid request content: required array of ids in JSON";
            }
        }

        [HttpPost("buy")]
        public async Task<object> BuyOrders()
        {
            string errorResponse;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorResponse, _sessionManager))
                return errorResponse;

            try
            {
                int[] idsToBuy = await HttpContext.Request.ReadFromJsonAsync<int[]>();
                var orders = new List<Order>();
                foreach (int orderId in idsToBuy)
                {
                    var order = await _ordersManager.FindOrderAsync(orderId, _context);
                    if (order != null)
                        orders.Add(order);
                }
                await _ordersManager.DeleteMultipleSoftAsync(orders, _context);
                HttpContext.Response.StatusCode = 204;
                return "{}";
            }
            catch (JsonException e)
            {
                HttpContext.Response.StatusCode = 400;
                return "Invalid request content required array of ids in JSON";
            }
        }
    }
}
