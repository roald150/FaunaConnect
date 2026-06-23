using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;

using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Services;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// SQLite
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                          ?? "Data Source=FaunaConnect2.db";
// EF Core
builder.Services.AddDbContext<FaunaDbContext>(options => options.UseSqlite(connectionString));

//services
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAnimalSpeciesService, AnimalSpeciesService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHuntingGroundService, HuntingGroundService>();
builder.Services.AddScoped<IDamageReportService, DamageReportService>();
builder.Services.AddScoped<IChatService, ChatService>();

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

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "FaunaConnect2 API",
        Description = "API for FaunaConnect2 - Hunting and Damage Registration System"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FaunaConnect2 API v1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// create and seed db
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FaunaDbContext>();
    context.Database.Migrate();

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

// routes
app.MapControllers();

app.Run();