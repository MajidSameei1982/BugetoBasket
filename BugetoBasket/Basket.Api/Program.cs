using Basket.Api.Infrastructure.Context;
using Basket.Api.Infrastructure.MappingProfile;
using Basket.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BasketDatabaseContext>(a =>
    a.UseSqlServer(builder.Configuration.GetConnectionString("BasketConnectionString")));

builder.Services.AddAutoMapper(typeof(BasketMappingProfile));

builder.Services.AddTransient<IBasketService, BasketService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();