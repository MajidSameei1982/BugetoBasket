using Basket.Api.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Basket.Api.Infrastructure.Context;

public class BasketDatabaseContext : DbContext
{
    public BasketDatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Model.Entities.Basket> Baskets { get; set; }

    public DbSet<BasketItem> BasketItems { get; set; }
}