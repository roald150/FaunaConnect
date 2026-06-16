using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Define the connection string for SQL Server
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                          ?? "Server=(localdb)\\mssqllocaldb;Database=FaunaConnect2Db;Trusted_Connection=True;";

// 2. Register EF Core in the application (so controllers can use the database)
builder.Services.AddDbContext<FaunaDbContext>(options => options.UseSqlServer(connectionString));

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

// 3. Ensure .NET understands we are using Controllers (like RegistrationsController)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// 4. Enable OpenAPI (useful for testing your endpoints!)
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // This allows you to view the API documentation in the browser during development
    app.MapOpenApi();
}
else
{
    // Use HTTPS redirection in production only
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// 5. Automatically create database and seed with test data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FaunaDbContext>();
    
    // Ensure SQL Server creates the database and tables if they don't exist
    context.Database.EnsureCreated(); 

    // If the database is empty, create initial accounts for the demo
    if (!context.Users.Any())
    {
        var hunter = new User { Name = "Roald the Hunter", Email = "roald@jacht.nl", PasswordHash = BCrypt.Net.BCrypt.HashPassword("welcome"), Role = "Hunter" };
        var admin = new User { Name = "Admin", Email = "admin@jachtveld.nl", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), Role = "Admin" };
        
        context.Users.AddRange(hunter, admin);
        context.SaveChanges(); 

        var farmer = new User 
        { 
            Name = "Farmer Harms", 
            Email = "harms@boerderij.nl", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("welcome"), 
            Role = "Farmer",
            LinkedHunterId = hunter.Id
        };
        context.Users.Add(farmer);
        context.SaveChanges();

        // Add animal species
        context.AnimalSpecies.AddRange(
            new AnimalSpecies { Name = "Roe Deer" },
            new AnimalSpecies { Name = "Wild Boar" },
            new AnimalSpecies { Name = "Hare" },
            new AnimalSpecies { Name = "Pheasant" }
        );

        // Add an initial registration linked to the hunter
        context.Registrations.Add(new Registration 
        { 
            AnimalName = "Roe Deer", 
            Latitude = 51.650, 
            Longitude = 5.050, 
            UserId = hunter.Id 
        });
        context.SaveChanges();
    }
}

// 6. Map routes to controllers (e.g., /api/registrations)
app.MapControllers();

app.Run();