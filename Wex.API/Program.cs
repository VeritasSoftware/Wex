using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Wex.API;
using Wex.API.Services;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment);

app.MapGet("/moneymanagement/{identifier}/{country?}", async (string identifier, string? country,
                                                    [FromServices] IMoneyManagementService moneyManagementService) =>
{
    country = string.IsNullOrWhiteSpace(country) ? null : country;
    var result = await moneyManagementService.GetTransactionAsync(identifier, country);
    return result;
})
.WithName("GetTransaction")
.WithOpenApi(op =>
{
    var p = op.Parameters[1];
    p = new OpenApiParameter
    {
        Name = p.Name,
        In = p.In,
        Description = p.Description,
        Required = false, // ✅ Now optional
        Schema = p.Schema
    };

    return op;
});

app.Run();
