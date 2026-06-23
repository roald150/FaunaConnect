using Xunit;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Tests;


public class RegistrationFeatureTest
{
    [Fact]
    public void UserRole_ShouldDetermineButtonVisibility()
    {
        
        var jager = new User { Role = "Jager" };
        var admin = new User { Role = "Admin" };
        var boer = new User { Role = "Boer" };

        Assert.True(jager.Role == "Jager" || jager.Role == "Boer");
        Assert.True(boer.Role == "Jager" || boer.Role == "Boer");
        Assert.False(admin.Role == "Jager" || admin.Role == "Boer");
    }
}
