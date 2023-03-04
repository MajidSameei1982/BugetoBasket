namespace Basket.Api.Model.Entities;

public class Basket
{
    public Basket(string userId)
    {
        UserId = userId;
    }

    public Basket()
    {
    }

    public Guid Id { get; set; }

    public string UserId { get; set; }

    public List<BasketItem> BasketItems { get; set; } = new();
}