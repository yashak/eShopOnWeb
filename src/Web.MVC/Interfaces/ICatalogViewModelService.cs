using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Services;

public interface ICatalogViewModelService
{
    Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
    Task<IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetBrands();
    Task<IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetTypes();
}
