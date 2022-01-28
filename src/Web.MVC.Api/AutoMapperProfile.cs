using AutoMapper;

namespace Web.MVC.Api;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap< BL.Entity.Basket, Dto.Basket>();
        CreateMap< BL.Entity.BasketItem, Dto.BasketItem>();
        CreateMap< BL.Entity.CatalogIndex, Dto.CatalogIndex>();
        CreateMap< BL.Entity.CatalogItem, Dto.CatalogItem>();
        CreateMap< BL.Entity.PaginationInfo, Dto.PaginationInfo>();
        CreateMap< BL.Entity.SelectListItem, Dto.SelectListItem>();

        CreateMap<Dto.Basket, BL.Entity.Basket>();
        CreateMap<Dto.BasketItem, BL.Entity.BasketItem>();
        CreateMap<Dto.CatalogIndex, BL.Entity.CatalogIndex>();
        CreateMap<Dto.CatalogItem, BL.Entity.CatalogItem>();
        CreateMap<Dto.PaginationInfo, BL.Entity.PaginationInfo>();
        CreateMap<Dto.SelectListItem, BL.Entity.SelectListItem>();
    }
}

