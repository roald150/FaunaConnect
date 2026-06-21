using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Controllers;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FaunaConnect2.Api.Tests;

public class AnimalSpeciesControllerTest
{
    [Fact]
    public async Task Get_ShouldReturnAllSpecies()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<FaunaDbContext>()
            .UseInMemoryDatabase(databaseName: "AnimalSpeciesList")
            .Options;

        using (var context = new FaunaDbContext(options))
        {
            context.AnimalSpecies.Add(new AnimalSpecies { Name = "Test Ree" });
            context.AnimalSpecies.Add(new AnimalSpecies { Name = "Test Zwijn" });
            await context.SaveChangesAsync();
        }

        using (var context = new FaunaDbContext(options))
        {
            IAnimalSpeciesService service = new AnimalSpeciesService(context);
            var controller = new AnimalSpeciesController(service);

            // Act
            var result = await controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AnimalSpeciesResource>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<AnimalSpeciesResource>>(actionResult.Value);
            Assert.Equal(2, list.Count());
        }
    }
}
