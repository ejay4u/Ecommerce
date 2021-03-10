using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        public void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Order()
                {
                    Id = 1,
                    CustomerId = 1,
                    Items = new List<OrderItem>()
                {
                    new OrderItem() { OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 10},
                    new OrderItem() { OrderId = 2, ProductId = 2, Quantity = 5, UnitPrice = 20},
                    new OrderItem() { OrderId = 2, ProductId = 3, Quantity = 8, UnitPrice = 40},
                    new OrderItem() { OrderId = 3, ProductId = 2, Quantity = 10, UnitPrice = 20},
                },
                    OrderDate = DateTime.Now,
                    Total = 200
                });
                dbContext.Orders.Add(new Order()
                {
                    Id = 2,
                    CustomerId = 2,
                    Items = new List<OrderItem>()
                {
                    new OrderItem() { OrderId = 4, ProductId = 1, Quantity = 10, UnitPrice = 10},
                    new OrderItem() { OrderId = 5, ProductId = 3, Quantity = 5, UnitPrice = 20},
                    new OrderItem() { OrderId = 6, ProductId = 1, Quantity = 8, UnitPrice = 40},
                    new OrderItem() { OrderId = 6, ProductId = 2, Quantity = 10, UnitPrice = 20},
                },
                    OrderDate = DateTime.Now,
                    Total = 100
                });
                dbContext.Orders.Add(new Order()
                {
                    Id = 3,
                    CustomerId = 3,
                    Items = new List<OrderItem>()
                {
                    new OrderItem() { OrderId = 7, ProductId = 1, Quantity = 10, UnitPrice = 10},
                    new OrderItem() { OrderId = 8, ProductId = 2, Quantity = 5, UnitPrice = 20},
                    new OrderItem() { OrderId = 8, ProductId = 3, Quantity = 8, UnitPrice = 40},
                    new OrderItem() { OrderId = 9, ProductId = 2, Quantity = 10, UnitPrice = 20},
                },
                    OrderDate = DateTime.Now,
                    Total = 300
                });
                dbContext.SaveChanges();
            }
        }
        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.Items)
                    .ToListAsync();

                if (orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Order>, 
                        IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {

                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
