using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Logging;

namespace Web.MVC.Services;

/// <summary>
/// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
/// with UI-specific types (view models and SelectListItem types).
/// </summary>
public class CatalogViewModelService : BaseApiService, ICatalogViewModelService
{
    private readonly ILogger<CatalogViewModelService> _logger;
    private readonly IUriComposer _uriComposer;

    public CatalogViewModelService(
        ILoggerFactory loggerFactory,
        IUriComposer uriComposer,
        IHttpClientFactory clientFactory): base(clientFactory)
    {
        _logger = loggerFactory.CreateLogger<CatalogViewModelService>();
        _uriComposer = uriComposer;
    }

    public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        _logger.LogInformation("GetCatalogItems called.");

        var filterSpecification = new CatalogFilterSpecification(brandId, typeId);
        var filterPaginatedSpecification =
            new CatalogFilterPaginatedSpecification(itemsPage * pageIndex, itemsPage, brandId, typeId);
        
        var catalogIndex = await _client.ItemsAsync(pageIndex, itemsPage, brandId, typeId);

        var vm = new CatalogIndexViewModel()
        {
            CatalogItems = catalogIndex.CatalogItems.Select(i => new CatalogItemViewModel()
            {
                Id = i.Id,
                Name = i.Name,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri),
                //TODO: fix
                Price = (decimal) i.Price
            }).ToList(),
            Brands = (await GetBrands()).ToList(),
            Types = (await GetTypes()).ToList(),
            BrandFilterApplied = brandId ?? 0,
            TypesFilterApplied = typeId ?? 0,
            PaginationInfo = new PaginationInfoViewModel()
            {
                ActualPage = catalogIndex.PaginationInfo.ActualPage,
                ItemsPerPage = catalogIndex.PaginationInfo.ItemsPerPage,
                TotalItems = catalogIndex.PaginationInfo.TotalItems,
                TotalPages = catalogIndex.PaginationInfo.TotalPages
            }
        };

        vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
        vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

        return vm;
    }

    public async Task<IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetBrands()
    {
        _logger.LogInformation("GetBrands called.");

        var brands = await _client.BrandsAsync();
        
        var items = brands
            .Select(brand => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = brand.Value, Text = brand.Text })
            .OrderBy(b => b.Text)
            .ToList();

        var allItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    public async Task<IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetTypes()
    {
        _logger.LogInformation("GetTypes called.");
        var types = await _client.TypesAsync();

        var items = types
            .Select(type => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = type.Value, Text = type.Text })
            .OrderBy(t => t.Text)
            .ToList();

        var allItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }
}
