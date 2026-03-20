using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Wex.API;
using Wex.API.Models;
using Wex.API.Services;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment);

// Minimal API endpoints
// Add card
app.MapPost("/moneymanagement/card", async (CardCreateModel card, [FromServices] IMoneyManagementService moneyManagementService) =>
{
    return await moneyManagementService.AddCardAsync(card);
})
.WithName("AddCard");

// Get balance by card identifier & country of currency
app.MapGet("/moneymanagement/card/balance/{identifier}/{country?}", async (string identifier, string? country,
                                                    [FromServices] IMoneyManagementService moneyManagementService) =>
{
    country = string.IsNullOrWhiteSpace(country) ? null : country;
    var result = await moneyManagementService.GetCardBalanceAsync(identifier, country);
    return result;
})
.WithName("GetCardBalance");

// Add transaction
app.MapPost("/moneymanagement/transaction", async (TransactionCreateModel transaction, [FromServices] IMoneyManagementService moneyManagementService) =>
{
    return await moneyManagementService.AddTransactionAsync(transaction);
})
.WithName("AddTransaction");

// Get transaction by transaction identifier & country of currency
app.MapGet("/moneymanagement/transaction/{identifier}/{country?}", async (string identifier, string? country,
                                                    [FromServices] IMoneyManagementService moneyManagementService) =>
{
    country = string.IsNullOrWhiteSpace(country) ? null : country;
    var result = await moneyManagementService.GetTransactionAsync(identifier, country);
    return result;
})
.WithName("GetTransaction");

app.Run();
