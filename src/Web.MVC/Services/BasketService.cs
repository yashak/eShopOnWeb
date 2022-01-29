using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Web.MVC.Services;

public class BasketService : IBasketService
{
    public Task<Basket> AddItemToBasket(string username, int catalogItemId, decimal price, int quantity = 1)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBasketAsync(int basketId)
    {
        throw new NotImplementedException();
    }

    public Task<Basket> SetQuantities(int basketId, Dictionary<string, int> quantities)
    {
        throw new NotImplementedException();
    }

    public Task TransferBasketAsync(string anonymousId, string userName)
    {
        throw new NotImplementedException();
    }
}
