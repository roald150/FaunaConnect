using FaunaConnect2.App.Helpers;

namespace FaunaConnect2.App.Tests;

public class InverseBoolConverterTests
{
    private readonly InverseBoolConverter _converter = new();

    [Fact]
    public void Convert_True_ReturnsFalse()
    {
        var result = _converter.Convert(true, typeof(bool), null, null);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_False_ReturnsTrue()
    {
        var result = _converter.Convert(false, typeof(bool), null, null);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonBool_ReturnsFalse()
    {
        var result = _converter.Convert("not a bool", typeof(bool), null, null);
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_True_ReturnsFalse()
    {
        var result = _converter.ConvertBack(true, typeof(bool), null, null);
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_False_ReturnsTrue()
    {
        var result = _converter.ConvertBack(false, typeof(bool), null, null);
        Assert.Equal(true, result);
    }
}
