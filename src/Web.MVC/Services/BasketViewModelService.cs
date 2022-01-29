using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Pages.Basket;

namespace Microsoft.eShopWeb.Web.Services;

public class BasketViewModelService : IBasketViewModelService
{
    public Task<int> CountTotalBasketItems(string username)
    {
        throw new NotImplementedException();
    }

    public Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
    {
        throw new NotImplementedException();
    }

    public Task<BasketViewModel> Map(Basket basket)
    {
        throw new NotImplementedException();
    }
}
