using AiPoweredCodex.Application;
using AiPoweredCodex.Infrastructure;
using AiPoweredCodex.Api.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AiPoweredCodex API",
        Version = "v1",
        Description = "Car marketplace API with token-based authentication."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only the token value. Swagger will send it as Bearer <token>."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();
await app.Services.SeedDemoDataAsync();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AiPoweredCodex API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "AiPoweredCodex Swagger";
});

app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "AiPoweredCodex Car Marketplace API",
    auth = "Custom JWT bearer token",
    swagger = "/swagger",
    sampleAccounts = new[]
    {
        new { email = "admin@aipoweredcodex.com", password = "Admin@123", role = "Admin" },
        new { email = "seller@aipoweredcodex.com", password = "Seller@123", role = "Seller" },
        new { email = "buyer@aipoweredcodex.com", password = "Buyer@123", role = "User" }
    }
}));

app.Run();
