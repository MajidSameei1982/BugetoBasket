using AutoMapper;
using Basket.Api.Model.Entities;
using Basket.Api.Services;

namespace Basket.Api.Infrastructure.MappingProfile;

public class BasketMappingProfile : Profile
{
    public BasketMappingProfile()
    {
        CreateMap<BasketItem, AddBasketItemDto>().ReverseMap();
    }
}