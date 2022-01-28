using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;

namespace Web.MVC.BL;

public interface IBasketService
{
    Task<Entity.Basket> GetOrCreateBasketForUser(string userName);
    Task<int> CountTotalBasketItems(string username);
    
    //Task<Entity.Basket> Map(Basket basket);
}
