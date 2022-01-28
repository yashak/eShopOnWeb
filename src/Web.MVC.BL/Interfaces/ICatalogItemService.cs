namespace Web.MVC.BL;

public interface ICatalogItemService
{
    Task UpdateCatalogItem(Entity.CatalogItem viewModel);
}
