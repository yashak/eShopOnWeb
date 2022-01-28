using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.Extensions.Logging;

namespace Web.MVC.BL;

/// <summary>
/// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
/// with UI-specific types (view models and SelectListItem types).
/// </summary>
public class CatalogService : ICatalogService
{
    private readonly ILogger<CatalogService> _logger;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;
    private readonly IUriComposer _uriComposer;

    public CatalogService(
        ILoggerFactory loggerFactory,
        IRepository<CatalogItem> itemRepository,
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository,
        IUriComposer uriComposer)
    {
        _logger = loggerFactory.CreateLogger<CatalogService>();
        _itemRepository = itemRepository;
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
        _uriComposer = uriComposer;
    }

    public async Task<Entity.CatalogIndex> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        _logger.LogInformation("GetCatalogItems called.");

        var filterSpecification = new CatalogFilterSpecification(brandId, typeId);
        var filterPaginatedSpecification =
            new CatalogFilterPaginatedSpecification(itemsPage * pageIndex, itemsPage, brandId, typeId);

        // the implementation below using ForEach and Count. We need a List.
        var itemsOnPage = await _itemRepository.ListAsync(filterPaginatedSpecification);
        var totalItems = await _itemRepository.CountAsync(filterSpecification);

        var vm = new Entity.CatalogIndex()
        {
            CatalogItems = itemsOnPage.Select(i => new Entity.CatalogItem()
            {
                Id = i.Id,
                Name = i.Name,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri),
                Price = i.Price
            }).ToList(),
            Brands = (await GetBrands()).ToList(),
            Types = (await GetTypes()).ToList(),
            BrandFilterApplied = brandId ?? 0,
            TypesFilterApplied = typeId ?? 0,
            PaginationInfo = new Entity.PaginationInfo()
            {
                ActualPage = pageIndex,
                ItemsPerPage = itemsOnPage.Count,
                TotalItems = totalItems,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / itemsPage)).ToString())
            }
        };

        vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
        vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

        return vm;
    }

    public async Task<IEnumerable<Entity.SelectListItem>> GetBrands()
    {
        _logger.LogInformation("GetBrands called.");
        var brands = await _brandRepository.ListAsync();

        var items = brands
            .Select(brand => new Entity.SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand })
            .OrderBy(b => b.Text)
            .ToList();

        var allItem = new Entity.SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    public async Task<IEnumerable<Entity.SelectListItem>> GetTypes()
    {
        _logger.LogInformation("GetTypes called.");
        var types = await _typeRepository.ListAsync();

        var items = types
            .Select(type => new Entity.SelectListItem() { Value = type.Id.ToString(), Text = type.Type })
            .OrderBy(t => t.Text)
            .ToList();

        var allItem = new Entity.SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }
}
