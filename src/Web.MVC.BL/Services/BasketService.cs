using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Web.MVC.BL;

public class BasketService : IBasketService
{
    private readonly IRepository<Basket> _basketRepository;
    private readonly IUriComposer _uriComposer;
    private readonly IBasketQueryService _basketQueryService;
    private readonly IRepository<CatalogItem> _itemRepository;

    public BasketService(IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IUriComposer uriComposer,
        IBasketQueryService basketQueryService)
    {
        _basketRepository = basketRepository;
        _uriComposer = uriComposer;
        _basketQueryService = basketQueryService;
        _itemRepository = itemRepository;
    }

    public async Task<Entity.Basket> GetOrCreateBasketForUser(string userName)
    {
        var basketSpec = new BasketWithItemsSpecification(userName);
        var basket = (await _basketRepository.GetBySpecAsync(basketSpec));

        if (basket == null)
        {
            return await CreateBasketForUser(userName);
        }
        var viewModel = await Map(basket);
        return viewModel;
    }

    private async Task<Entity.Basket> CreateBasketForUser(string userId)
    {
        var basket = new Basket(userId);
        await _basketRepository.AddAsync(basket);

        return new Entity.Basket()
        {
            BuyerId = basket.BuyerId,
            Id = basket.Id,
        };
    }

    private async Task<List<Entity.BasketItem>> GetBasketItems(IReadOnlyCollection<BasketItem> basketItems)
    {
        var catalogItemsSpecification = new CatalogItemsSpecification(basketItems.Select(b => b.CatalogItemId).ToArray());
        var catalogItems = await _itemRepository.ListAsync(catalogItemsSpecification);

        var items = basketItems.Select(basketItem =>
        {
            var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);

            var BasketItem = new Entity.BasketItem
            {
                Id = basketItem.Id,
                UnitPrice = basketItem.UnitPrice,
                Quantity = basketItem.Quantity,
                CatalogItemId = basketItem.CatalogItemId,
                PictureUrl = _uriComposer.ComposePicUri(catalogItem.PictureUri),
                ProductName = catalogItem.Name
            };
            return BasketItem;
        }).ToList();

        return items;
    }

    private async Task<Entity.Basket> Map(Basket basket)
    {
        return new Entity.Basket()
        {
            BuyerId = basket.BuyerId,
            Id = basket.Id,
            Items = await GetBasketItems(basket.Items)
        };
    }

    public async Task<int> CountTotalBasketItems(string username)
    {
        var counter = await _basketQueryService.CountTotalBasketItems(username);

        return counter;
    }
}
