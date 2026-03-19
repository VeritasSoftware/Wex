using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Globalization;
using Wex.API.Entities;
using Wex.API.Repositories;
using Wex.API.Services;

namespace Wex.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(config => config.AddConsole());

            services.AddOpenApi();
            //Swagger
            services.AddEndpointsApiExplorer(); // Required for Minimal APIs
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Money Management Minimal API",
                    Description = "AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.",
                    Version = "v1"
                });
            });

            services.AddDbContext<MoneyManagementContext>(b => b.UseSqlite("Data Source=appdata.db"));
            services.AddHttpClient<IHttpService, HttpService>();
            services.AddScoped<IMoneyManagementRepository, MoneyManagementRepository>();
            services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
            services.AddScoped<IMoneyManagementService, MoneyManagementService>();
        }

        //public void Configure(IApplicationBuilder app, WebApplication webApplication)
        public void Configure(WebApplication app, IWebHostEnvironment environment)
        {
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
        }

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
    }
}
