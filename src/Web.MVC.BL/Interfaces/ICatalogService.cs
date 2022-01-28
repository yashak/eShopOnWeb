using Web.MVC.BL.Entity;

namespace Web.MVC.BL;

public interface ICatalogService
{
    Task<CatalogIndex> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
    Task<IEnumerable<SelectListItem>> GetBrands();
    Task<IEnumerable<SelectListItem>> GetTypes();
}
