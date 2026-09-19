using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Entity.Test.Players;

public class UsernameTest
{
    [Fact]
    public void Username_Valid() {
        // arrange
        var value = "hiæøÅ1278";

        // act
        var username = new Username(value);

        // assert
        Assert.Equal(username.Value, value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hei ho")]
    [InlineData("j*n")]
    [InlineData("=ndwnfks")]
    [InlineData("tretten123456")]
    public void Username_Invalid(string value)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new Username(value));
    }
}
