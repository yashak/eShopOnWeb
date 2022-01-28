using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Web.MVC.BL;

public class CatalogItemService : ICatalogItemService
{
    private readonly IRepository<CatalogItem> _catalogItemRepository;

    public CatalogItemService(IRepository<CatalogItem> catalogItemRepository)
    {
        _catalogItemRepository = catalogItemRepository;
    }

    public async Task UpdateCatalogItem(Entity.CatalogItem viewModel)
    {
        var existingCatalogItem = await _catalogItemRepository.GetByIdAsync(viewModel.Id);
        existingCatalogItem.UpdateDetails(viewModel.Name, existingCatalogItem.Description, viewModel.Price);
        await _catalogItemRepository.UpdateAsync(existingCatalogItem);
    }
}
