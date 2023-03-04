using Basket.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    // GET: api/Basket
    [HttpGet]
    public IActionResult Get(string userId)
    {
        var basket = _basketService.GetBasket(userId);

        return Ok(basket);
    }

    // POST: api/Basket
    [HttpPost]
    public IActionResult Post(AddBasketItemDto addBasketItemDto, string userId)
    {
        var basket = _basketService.GetOrCreateBasketForUser(userId);

        addBasketItemDto.BasketId = basket.Id;

        _basketService.AddItemToBasket(addBasketItemDto);

        var basketData = _basketService.GetBasket(userId);

        return Ok();
    }

    [HttpPut]
    public IActionResult SetQuantity(Guid basketItemId, int quantity)
    {
        _basketService.SetQuantities(basketItemId, quantity);

        return Ok();
    }

    // DELETE: api/Basket/5
    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        _basketService.Delete(id);

        return Ok();
    }
}