namespace Web.MVC.BL.Entity;
public class CatalogIndex
{
    public List<CatalogItem> CatalogItems { get; set; }
    public List<SelectListItem> Brands { get; set; }
    public List<SelectListItem> Types { get; set; }
    public int? BrandFilterApplied { get; set; }
    public int? TypesFilterApplied { get; set; }
    public PaginationInfo PaginationInfo { get; set; }
}
