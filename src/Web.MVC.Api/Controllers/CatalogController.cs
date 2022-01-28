using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.BL;


namespace Web.MVC.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    ICatalogService _catalogService;
    private readonly IMapper _mapper;

    public CatalogController(IMapper mapper, ICatalogService catalogService)
    {
        _mapper = mapper;
        _catalogService = catalogService;
    }

    [HttpGet("brands")]
    public async Task<IEnumerable<Dto.SelectListItem>> GetBrands()
    {
        var result = await _catalogService.GetBrands();
        return _mapper.Map<IEnumerable<Dto.SelectListItem>>(result);
    }

    [HttpGet("types")]
    public async Task<IEnumerable<Dto.SelectListItem>> GetTypes()
    {
        var result = await _catalogService.GetTypes();
        return _mapper.Map<IEnumerable<Dto.SelectListItem>>(result);
    }

    [HttpGet("items")]
    public async Task<Dto.CatalogIndex> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId) {
        var result = await _catalogService.GetCatalogItems(pageIndex, itemsPage, brandId, typeId);
        return _mapper.Map<Dto.CatalogIndex>(result);
    }
}
