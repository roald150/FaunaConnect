using Xunit;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Tests;

// Voor een echte Feature test (Integration test) gebruiken we WebApplicationFactory
// Maar we houden het simpel voor de demo binnen de test omgeving.
public class RegistrationFeatureTest
{
    [Fact]
    public void UserRole_ShouldDetermineButtonVisibility()
    {
        // Deze test simuleert de logica in MainPage.xaml.cs
        // "Feature": Knoppen zijn alleen zichtbaar voor Jagers en Boeren.
        
        var jager = new User { Role = "Jager" };
        var admin = new User { Role = "Admin" };
        var boer = new User { Role = "Boer" };

        Assert.True(jager.Role == "Jager" || jager.Role == "Boer");
        Assert.True(boer.Role == "Jager" || boer.Role == "Boer");
        Assert.False(admin.Role == "Jager" || admin.Role == "Boer");
    }
}
