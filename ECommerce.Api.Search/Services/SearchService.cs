using ECommerce.Api.Search.Interfaces;
using ECommerce.Api.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customersService = customersService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var customerResult = await customersService.GetCustomersAsync(customerId);
            if(ordersResult.IsSuccess)
            {
                foreach (var order in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ? productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name : 
                            "Product information is not available";
                    }

                }
                var result = new
                {
                    Customer = customerResult.IsSuccess ? new Customer { Id = customerResult.Customer.Id, Name = customerResult.Customer.Name, Address = customerResult.Customer.Address } :
                    new Customer { Name = "Not available", Address = "Not available" },
                    Orders = ordersResult.Orders
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
