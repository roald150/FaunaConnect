using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. De verbindingsreeks (ConnectionString) voor SQL Server definiëren
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                          ?? "Server=(localdb)\\mssqllocaldb;Database=FaunaConnect2Db;Trusted_Connection=True;";

// 2. EF Core registreren in de applicatie (zodat de controllers de database kunnen gebruiken)
builder.Services.AddDbContext<FaunaDbContext>(options => options.UseSqlServer(connectionString));

// 3. Zorg dat .NET begrijpt dat we Controllers (zoals RegistrationsController) gebruiken
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// 4. Dit stond er al in: OpenAPI inschakelen (handig voor het testen van je endpoints!)
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure het HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Dit zorgt ervoor dat je de API-documentatie kunt bekijken in de browser tijdens het ontwerpen
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 5. Database automatisch aanmaken en vullen met testgegevens (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FaunaDbContext>();
    
    // Zorgt ervoor dat SQL Server de database en tabellen aanmaakt als ze nog niet bestaan
    context.Database.EnsureCreated(); 

    // Als de database helemaal leeg is, maken we alvast accounts aan voor je demo
    if (!context.Users.Any())
    {
        var jager = new User { Name = "Roald de Jager", Email = "roald@jacht.nl", Role = "Jager" }; //
        var boer = new User { Name = "Boer Harms", Email = "harms@boerderij.nl", Role = "Boer" };   //
        
        context.Users.AddRange(jager, boer);
        context.SaveChanges(); 

        // Voeg direct een eerste waarneming toe gekoppeld aan de jager
        context.Registrations.Add(new Registration 
        { 
            AnimalName = "Ree", 
            Latitude = 51.650, 
            Longitude = 5.050, 
            UserId = jager.Id 
        });
        context.SaveChanges();
    }
}

// 6. Vertel de app dat hij de routes naar je Controllers moet mappen (bijv. /api/registrations)
app.MapControllers();

app.Run();