using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.BL;

namespace Web.MVC.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(IMapper mapper, IBasketService basketService) {
        _mapper = mapper;
        _basketService = basketService;
    }

    [HttpGet("{userName}")]
    public async Task<Dto.Basket> Get(string userName)
    {
        var result = await _basketService.GetOrCreateBasketForUser(userName);
        return _mapper.Map<Dto.Basket>(result);
    }

    [HttpGet("{userName}/items-count")]
    public async Task<int> CountTotalBasketItems(string username) {
        return await _basketService.CountTotalBasketItems(username);
    }
}
