using FaunaConnect2.App.Services;
using FaunaConnect2.App.ViewModels;
using Moq;

namespace FaunaConnect2.App.Tests;

public class RegisterViewModelValidationTests
{
    private readonly Mock<IDeviceService> _deviceServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    [Fact]
    public async Task Register_EmptyName_ShowsWarning()
    {
        var vm = new RegisterViewModel(_userServiceMock.Object, _deviceServiceMock.Object);
        vm.Name = "";
        vm.Email = "test@example.com";
        vm.Password = "password123";

        await vm.RegisterCommand.ExecuteAsync(null);

        _deviceServiceMock.Verify(
            d => d.DisplayAlertAsync("Warning", "Please fill in all fields.", "OK"),
            Times.Once);
    }

    [Fact]
    public async Task Register_EmptyEmail_ShowsWarning()
    {
        var vm = new RegisterViewModel(_userServiceMock.Object, _deviceServiceMock.Object);
        vm.Name = "Test User";
        vm.Email = "";
        vm.Password = "password123";

        await vm.RegisterCommand.ExecuteAsync(null);

        _deviceServiceMock.Verify(
            d => d.DisplayAlertAsync("Warning", "Please fill in all fields.", "OK"),
            Times.Once);
    }

    [Fact]
    public async Task Register_EmptyPassword_ShowsWarning()
    {
        var vm = new RegisterViewModel(_userServiceMock.Object, _deviceServiceMock.Object);
        vm.Name = "Test User";
        vm.Email = "test@example.com";
        vm.Password = "";

        await vm.RegisterCommand.ExecuteAsync(null);

        _deviceServiceMock.Verify(
            d => d.DisplayAlertAsync("Warning", "Please fill in all fields.", "OK"),
            Times.Once);
    }
}
