using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using DataAccessLayer.Data;

namespace BuisnessLayer
{
    public class OrdersManager
    {
        public IEnumerable<Order> GetOrders(int userId, ApplicationDbContext context)
        {
            var orders = context.Orders.Where(o => o.UserId == userId && o.Status == OrderStatus.Name(OrderStatuses.Active));
            return orders.ToList();
        }

        public async Task<Order> CreateOrderAsync(int userId, int productId, int amount, ApplicationDbContext context)
        {
            var newOrder = new Order() 
            { ProductId = productId, UserId = userId, Amount = amount, AddingDate = DateTime.Now, Status = OrderStatus.Name(OrderStatuses.Active) };

            context.Add(newOrder);
            await context.SaveChangesAsync();
            return newOrder;
        }

        public async Task<Order> IncreaseOrderAmount(Order order, int additionalAmount, ApplicationDbContext context)
        {
            order.Amount+= additionalAmount;
            context.Update(order);
            await context.SaveChangesAsync();
            return order;
        }

        public Order FindOrder(int userId, int productId, ApplicationDbContext context)
        {
            var order = context.Orders.Where(o => o.UserId == userId && o.ProductId == productId && 
                o.Status == OrderStatus.Name(OrderStatuses.Active)).FirstOrDefault();
            return order;
        }

        public Order FindOrder(int orderId, ApplicationDbContext context)
        {
            var order = context.Orders.Where(o => o.Id == orderId && 
                o.Status == OrderStatus.Name(OrderStatuses.Active)).FirstOrDefault();
            return order;
        }

        public async Task<Order> UpdateOrderFromViewModelAsync(OrderViewModel model, ApplicationDbContext context)
        {
            var order = FindOrder(model.OrderId, context);
            if (order == null)
                throw new ArgumentException($"Invalid order id: {model.OrderId}");
            order.Amount = model.Amount;
            context.Update(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> UpdateOrdersAsync(OrderViewModel[] orderInfos, ApplicationDbContext context)
        {
            foreach(var orderInfo in orderInfos)
            {
                var order = FindOrder(orderInfo.OrderId, context);
                if (order == null)
                    throw new ArgumentException($"Invalid order id: {orderInfo.OrderId}");
                order.Amount = orderInfo.Amount;
                context.Update(order);
            }
            await context.SaveChangesAsync();
            return context.Orders;
        }

        public OrderViewModel CreateViewModelFromOrder(Order order, ApplicationDbContext context)
        {
            var model = new OrderViewModel()
            {
                AddingDate = order.AddingDate,
                Amount = order.Amount,
                OrderId = order.Id,
                GameName = context.Products.Where(p => p.Id == order.ProductId).First().Name
            };
            return model;
        }

        public List<OrderViewModel> CreateViewModelsListFromOrders(IEnumerable<Order> orders, ApplicationDbContext context)
        {
            var models = new List<OrderViewModel>();
            foreach(var order in orders)
            {
                models.Add(new OrderViewModel()
                {
                    AddingDate = order.AddingDate,
                    Amount = order.Amount,
                    OrderId = order.Id,
                    GameName = context.Products.Where(p => p.Id == order.ProductId).First().Name
                });
            }
            return models;
        }

        public async Task DeleteHardAsync(Order order, ApplicationDbContext context)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Order order, ApplicationDbContext context)
        {
            order.Status = OrderStatus.Name(OrderStatuses.Deleted);
            context.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMultipleHardAsync(IEnumerable<Order> orders, ApplicationDbContext context)
        {
            foreach(var order in orders)
                context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMultipleSoftAsync(IEnumerable<Order> orders, ApplicationDbContext context)
        {
            foreach (var order in orders)
            {
                order.Status = OrderStatus.Name(OrderStatuses.Deleted);
                context.Update(order);
            }
            await context.SaveChangesAsync();
        }

        public async Task BuyOrders(IEnumerable<Order> orders, ApplicationDbContext context)
        {
            foreach (var order in orders)
            {
                order.Status = OrderStatus.Name(OrderStatuses.Bought);
                context.Update(order);
            }
            await context.SaveChangesAsync();
        }
    }
}
