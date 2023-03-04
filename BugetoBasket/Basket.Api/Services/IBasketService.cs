using AutoMapper;
using Basket.Api.Infrastructure.Context;
using Basket.Api.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Basket.Api.Services;

public interface IBasketService
{
    BasketDto GetOrCreateBasketForUser(string userId);

    BasketDto GetBasket(string userId);

    void AddItemToBasket(AddBasketItemDto addBasketItemDto);

    void Delete(Guid basketItemId);

    void SetQuantities(Guid basketItemId, int quantity);

    void TransferBasket(string anonymousId, string userId);
}

public class BasketService : IBasketService
{
    private readonly BasketDatabaseContext _context;
    private readonly IMapper _mapper;

    public BasketService(BasketDatabaseContext context
        , IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public BasketDto GetOrCreateBasketForUser(string userId)
    {
        var basket = _context.Baskets
            .Include(i => i.BasketItems)
            .FirstOrDefault(s => s.UserId == userId);

        if (basket == null) return CreateBasketForUser(userId);

        return new BasketDto(basket.UserId)
        {
            Id = basket.Id,
            BasketItemDtos = basket.BasketItems.Select(s => new BasketItemDto
            {
                ProductId = s.ProductId,
                Id = s.Id,
                ProductName = s.ProductName,
                Quantity = s.Quantity,
                UnitPrice = s.UnitPrice,
                ImageUrl = s.ImageUrl
            }).ToList()
        };
    }

    public BasketDto GetBasket(string userId)
    {
        var basket = _context.Baskets
            .Include(i => i.BasketItems)
            .SingleOrDefault(s => s.UserId == userId);

        if (basket == null) return new BasketDto();

        return new BasketDto(basket.UserId)
        {
            Id = basket.Id,
            BasketItemDtos = basket.BasketItems.Select(s => new BasketItemDto
            {
                ProductId = s.ProductId,
                Id = s.Id,
                ProductName = s.ProductName,
                Quantity = s.Quantity,
                UnitPrice = s.UnitPrice,
                ImageUrl = s.ImageUrl
            }).ToList()
        };
    }

    public void AddItemToBasket(AddBasketItemDto addBasketItemDto)
    {
        var basket = _context.Baskets.FirstOrDefault(f => f.Id == addBasketItemDto.BasketId);

        if (basket == null) throw new Exception("Basket not found...!");

        var basketItem = _mapper.Map<BasketItem>(addBasketItemDto);

        basket.BasketItems.Add(basketItem);

        _context.SaveChanges();
    }

    public void Delete(Guid basketItemId)
    {
        var basketItem = _context.BasketItems.Find(basketItemId);

        if (basketItem == null) throw new Exception("BasketItem not found");

        _context.BasketItems.Remove(basketItem);

        _context.SaveChanges();
    }

    public void SetQuantities(Guid basketItemId, int quantity)
    {
        var basketItem = _context.BasketItems.Find(basketItemId);

        if (basketItem == null) throw new Exception("BasketItem not found");

        basketItem.SetQuantity(quantity);

        _context.SaveChanges();
    }

    public void TransferBasket(string anonymousId, string userId)
    {
        var anonymousBasket = _context.Baskets
            .Include(i => i.BasketItems)
            .SingleOrDefault(s => s.UserId == userId);

        if (anonymousBasket == null) return;

        var userBasket = _context.Baskets.SingleOrDefault(s => s.UserId == userId);

        if (userBasket != null) return;
        {
            userBasket = new Model.Entities.Basket(userId);

            _context.Baskets.Add(userBasket);

            _context.SaveChanges();
        }

        for (var i = 0; i < anonymousBasket.BasketItems.Count; i++)
        {
            var basketItem = anonymousBasket.BasketItems[i];

            userBasket.BasketItems.Add(
                new BasketItem
                {
                    ImageUrl = basketItem.ImageUrl,
                    ProductName = basketItem.ProductName,
                    Quantity = basketItem.Quantity,
                    UnitPrice = basketItem.UnitPrice
                }
            );
        }

        _context.Baskets.Remove(anonymousBasket);

        _context.SaveChanges();
    }

    private BasketDto CreateBasketForUser(string userId)
    {
        var basket = new Model.Entities.Basket(userId);

        _context.Baskets.Add(basket);

        _context.SaveChanges();

        return new BasketDto(basket.UserId)
        {
            Id = basket.Id
        };
    }
}

public class BasketDto
{
    public BasketDto(string userId)
    {
        UserId = userId;
    }

    public BasketDto()
    {
    }
    
    public Guid Id { get; set; }

    public string UserId { get; }

    public List<BasketItemDto> BasketItemDtos { get; set; }

    public int Total()
    {
        if (BasketItemDtos.Count > 0)
        {
            var total = BasketItemDtos.Sum(p => p.UnitPrice
                                                * p.Quantity);

            return total;
        }

        return 0;
    }
}

public class BasketItemDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; }

    public int UnitPrice { get; set; }

    public int Quantity { get; set; }

    public string ImageUrl { get; set; }

    public Guid BasketId { get; set; }
}

public class AddBasketItemDto
{
    public Guid BasketId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; }

    public int UnitPrice { get; set; }

    public int Quantity { get; set; }

    public string ImageUrl { get; set; }
}