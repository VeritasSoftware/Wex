using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Globalization;
using Wex.API.Entities;
using Wex.API.Repositories;
using Wex.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//Swagger
builder.Services.AddEndpointsApiExplorer(); // Required for Minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Money Management Minimal API",
        Description = "AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.",
        Version = "v1"
    });
});

builder.Services.AddDbContext<MoneyManagementContext>(b => b.UseSqlite("Data Source=appdata.db"));
builder.Services.AddHttpClient<IHttpService, HttpService>();
builder.Services.AddScoped<IMoneyManagementRepository, MoneyManagementRepository>();
builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
builder.Services.AddScoped<IMoneyManagementService, MoneyManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Money Management Minimal API V1"); 
    });
}

app.UseHttpsRedirection();

SeedDatabase(app);

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/moneymanagement/{identifier}/{country}", async (string identifier, string country,
                                                    [FromServices] IMoneyManagementService moneyManagementService) =>
{
    var result = await moneyManagementService.GetTransactionAsync(identifier, country);
    return result;
})
.WithName("GetTransaction");



app.Run();

static void SeedDatabase(WebApplication app)
{
    var serviceProvider = app.Services;
    using var scope = serviceProvider.CreateScope();

    var scopedServices = scope.ServiceProvider;
    var context = scopedServices.GetRequiredService<MoneyManagementContext>();

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    context.Database.OpenConnection();
    
    context.Cards.Add(new Card
    {
        Id = 1,
        CreditLimit = 1000.50m
    });

    context.SaveChanges();

    context.Transactions.Add(new Transaction
    {
        Id = 1,
        Amount = 100.00m,
        Date = DateOnly.FromDateTime(DateTime.Parse("16-03-2026", null, DateTimeStyles.AssumeLocal).ToUniversalTime()),
        Description = "Test Transaction",
        CardId = 1,
        Identifier = Guid.Parse("482acfc4-2aca-49b7-bfaa-14c92ad99d83")
    });

    context.SaveChanges();

    context.Transactions.Add(new Transaction
    {
        Id = 2,
        Amount = 200.00m,
        Date = DateOnly.FromDateTime(DateTime.Parse("17-03-2026", null, DateTimeStyles.AssumeLocal).ToUniversalTime()),
        Description = "Another Test Transaction",
        CardId = 1,
        Identifier = Guid.Parse("3d142c0f-1fc9-48cf-be15-26eea3497b71")
    });

    context.SaveChanges();

    context.Database.CloseConnection();
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
