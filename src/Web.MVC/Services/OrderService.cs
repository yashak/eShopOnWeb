using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Web.MVC.Services;

public class OrderService : IOrderService
{
    public Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        throw new NotImplementedException();
    }
}
