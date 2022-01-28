using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.BL;


namespace Web.MVC.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CatalogItemController : ControllerBase
{
    ICatalogItemService _catalogItemService;
    private readonly IMapper _mapper;

    public CatalogItemController(IMapper mapper, ICatalogItemService catalogItemService)
    {
        _mapper = mapper;
        _catalogItemService = catalogItemService;
    }


    [HttpPut]
    public async Task Put(Dto.CatalogItem catalogItem)
    {
        var item = _mapper.Map<BL.Entity.CatalogItem>(catalogItem);
        await _catalogItemService.UpdateCatalogItem(item);        
    }
}
