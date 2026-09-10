using Winvim.Automation;
using Xunit;

namespace Winvim.Tests;

public class HintCodeGeneratorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(81)]
    [InlineData(200)]
    public void Generate_ReturnsExactlyCountCodes(int count)
    {
        var codes = HintCodeGenerator.Generate(count);
        Assert.Equal(count, codes.Count);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(81)]
    [InlineData(200)]
    public void Generate_CodesAreUniqueAndFixedWidth(int count)
    {
        var codes = HintCodeGenerator.Generate(count);

        Assert.Equal(codes.Count, codes.Distinct().Count());

        if (codes.Count == 0) return;
        int expectedWidth = codes[0].Length;
        Assert.All(codes, code => Assert.Equal(expectedWidth, code.Length));
    }

    [Fact]
    public void Generate_NoCodeIsAPrefixOfAnother()
    {
        var codes = HintCodeGenerator.Generate(150);

        foreach (var a in codes)
        foreach (var b in codes)
        {
            if (a == b) continue;
            Assert.False(b.StartsWith(a), $"'{a}' is a prefix of '{b}'");
        }
    }
}
