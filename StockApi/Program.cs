using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StockApi.configurations;
using StockApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings") // 🔹 FIXED: Correct section
);

// Get MongoDB connection string
var mongoSettings = builder.Configuration.GetSection("MongoSettings").Get<MongoSettings>();
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

// Register MongoDB services
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(database);

//Regsitering thhe repositories
builder.Services.AddScoped<ProductRepository>();

// Add OpenAPI documentation (Swagger)
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();


