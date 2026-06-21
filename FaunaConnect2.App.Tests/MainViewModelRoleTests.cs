using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;
using FaunaConnect2.App.ViewModels;
using Moq;

namespace FaunaConnect2.App.Tests;

public class MainViewModelRoleTests
{
    private readonly Mock<IDeviceService> _deviceServiceMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();

    [Fact]
    public async Task Initialize_HunterRole_ShowsActionButtons()
    {
        var user = new User { Name = "Jager Piet", Role = "Hunter" };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(u => u.CurrentUser).Returns(user);
        userServiceMock.Setup(u => u.GetAuthenticatedClient()).ThrowsAsync(new Exception("No network"));

        var vm = new MainViewModel(userServiceMock.Object, _deviceServiceMock.Object, _serviceProviderMock.Object);
        await vm.Initialize();

        Assert.Equal("Welcome, Jager Piet", vm.WelcomeMessage);
        Assert.True(vm.IsActionButtonsVisible);
    }

    [Fact]
    public async Task Initialize_FarmerRole_ShowsActionButtons()
    {
        var user = new User { Name = "Boer Kees", Role = "Farmer" };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(u => u.CurrentUser).Returns(user);
        userServiceMock.Setup(u => u.GetAuthenticatedClient()).ThrowsAsync(new Exception("No network"));

        var vm = new MainViewModel(userServiceMock.Object, _deviceServiceMock.Object, _serviceProviderMock.Object);
        await vm.Initialize();

        Assert.True(vm.IsActionButtonsVisible);
    }

    [Fact]
    public async Task Initialize_AdminRole_HidesActionButtons()
    {
        var user = new User { Name = "Admin Jan", Role = "Admin" };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(u => u.CurrentUser).Returns(user);
        userServiceMock.Setup(u => u.GetAuthenticatedClient()).ThrowsAsync(new Exception("No network"));

        var vm = new MainViewModel(userServiceMock.Object, _deviceServiceMock.Object, _serviceProviderMock.Object);
        await vm.Initialize();

        Assert.Equal("Welcome, Admin Jan", vm.WelcomeMessage);
        Assert.False(vm.IsActionButtonsVisible);
    }
}
