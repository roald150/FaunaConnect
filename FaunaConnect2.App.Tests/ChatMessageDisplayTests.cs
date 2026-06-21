using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App.Tests;

public class ChatMessageDisplayTests
{
    [Fact]
    public void Alignment_IsFromMeTrue_ReturnsEnd()
    {
        var msg = new ChatViewModel.ChatMessageDisplay { IsFromMe = true };
        Assert.Equal(LayoutOptions.End, msg.Alignment);
    }

    [Fact]
    public void Alignment_IsFromMeFalse_ReturnsStart()
    {
        var msg = new ChatViewModel.ChatMessageDisplay { IsFromMe = false };
        Assert.Equal(LayoutOptions.Start, msg.Alignment);
    }

    [Fact]
    public void BgColor_IsFromMeTrue_ReturnsLightGreen()
    {
        var msg = new ChatViewModel.ChatMessageDisplay { IsFromMe = true };
        Assert.Equal(Color.FromArgb("#DCF8C6"), msg.BgColor);
    }

    [Fact]
    public void BgColor_IsFromMeFalse_ReturnsLightGray()
    {
        var msg = new ChatViewModel.ChatMessageDisplay { IsFromMe = false };
        Assert.Equal(Color.FromArgb("#EAEAEA"), msg.BgColor);
    }
}
