using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StockApi.configurations;
using StockApi.DbInit;
using StockApi.Interfaces;
using StockApi.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings")
);

var mongoSettings = builder.Configuration.GetSection("MongoSettings").Get<MongoSettings>();
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

// Registering MongoDB services
builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

// Registering the repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

//  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer YOUR_JWT_TOKEN' without quotes",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Stock API", Version = "v1" });

    c.DocInclusionPredicate((docName, apiDesc) => true);

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
#pragma warning disable CS8604 // Possible null reference argument.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
#pragma warning restore CS8604 // Possible null reference argument.
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/", () => "Welcome to the API");



using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.Run();
