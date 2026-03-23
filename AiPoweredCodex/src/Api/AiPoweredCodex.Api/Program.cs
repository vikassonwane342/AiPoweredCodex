using AiPoweredCodex.Application;
using AiPoweredCodex.Infrastructure;
using AiPoweredCodex.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.SeedDemoDataAsync();

app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "AiPoweredCodex Car Marketplace API",
    auth = "Custom JWT bearer token",
    sampleAccounts = new[]
    {
        new { email = "admin@aipoweredcodex.com", password = "Admin@123", role = "Admin" },
        new { email = "seller@aipoweredcodex.com", password = "Seller@123", role = "Seller" },
        new { email = "buyer@aipoweredcodex.com", password = "Buyer@123", role = "User" }
    }
}));

app.Run();
