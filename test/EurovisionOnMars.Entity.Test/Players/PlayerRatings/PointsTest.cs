using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Test.Players.PlayerRatings;

public class PointsTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    public void Create_WithValidValue_ReturnsPoints(int value)
    {
        var points = new Points(value);

        Assert.Equal(value, points.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    [InlineData(11)]
    [InlineData(13)]
    [InlineData(-1)]
    public void Create_WithInvalidValue_ThrowsException(int value)
    {
        Assert.Throws<ArgumentException>(() => new Points(value));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void IsSpecial_WithSpecialPoints_ReturnsTrue(int value)
    {
        var points = new Points(value);

        Assert.True(points.IsSpecial);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void IsSpecial_WithNonSpecialPoints_ReturnsFalse(int value)
    {
        var points = new Points(value);

        Assert.False(points.IsSpecial);
    }
}
