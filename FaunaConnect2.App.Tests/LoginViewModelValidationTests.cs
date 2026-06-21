using FaunaConnect2.App.Services;
using FaunaConnect2.App.ViewModels;
using Moq;

namespace FaunaConnect2.App.Tests;

public class LoginViewModelValidationTests
{
    private readonly Mock<IDeviceService> _deviceServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();

    [Fact]
    public async Task Login_EmptyEmail_ShowsWarning()
    {
        var vm = new LoginViewModel(_userServiceMock.Object, _deviceServiceMock.Object, _serviceProviderMock.Object);
        vm.Email = "";
        vm.Password = "somepassword";

        await vm.LoginCommand.ExecuteAsync(null);

        _deviceServiceMock.Verify(
            d => d.DisplayAlertAsync("Warning", "Please enter email and password.", "OK"),
            Times.Once);
    }

    [Fact]
    public async Task Login_EmptyPassword_ShowsWarning()
    {
        var vm = new LoginViewModel(_userServiceMock.Object, _deviceServiceMock.Object, _serviceProviderMock.Object);
        vm.Email = "test@example.com";
        vm.Password = "";

        await vm.LoginCommand.ExecuteAsync(null);

        _deviceServiceMock.Verify(
            d => d.DisplayAlertAsync("Warning", "Please enter email and password.", "OK"),
            Times.Once);
    }

    [Fact]
    public async Task Login_WhitespaceFields_ShowsWarning()
    {
        var vm = new LoginViewModel(_userServiceMock.Object, _deviceServiceMock.Object, _serviceProviderMock.Object);
        vm.Email = "   ";
        vm.Password = "   ";

        await vm.LoginCommand.ExecuteAsync(null);

        _deviceServiceMock.Verify(
            d => d.DisplayAlertAsync("Warning", "Please enter email and password.", "OK"),
            Times.Once);
    }
}
