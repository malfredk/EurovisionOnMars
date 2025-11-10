namespace EurovisionOnMars.Entity.Test;

public class CountryTest
{
    [Theory]
    [InlineData("østerrike", 1)]
    [InlineData("san marino", 26)]
    [InlineData("bosnia-hercegovina", 13)]
    public void Country_Valid(string name, int number)
    {
        // acts
        var country = new Country(number, name);

        // assert
        Assert.Equal(name, country.Name);
        Assert.Equal(number, country.Number);
    }

    [Theory]
    [InlineData("Danmark", 1)]
    [InlineData("england", 1)]
    [InlineData("danmark2", 26)]
    [InlineData("danmark", 0)]
    [InlineData("danmark", -10)]
    [InlineData("danmark", 27)]
    [InlineData("danmark_", 1)]
    [InlineData("", 1)]
    public void Country_Invalid(string name, int number)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new Country(number, name));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(26)]
    public void SetActualRank_Valid(int rank)
    {
        // arrange
        var country = new Country(5, "norge");

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(27)]
    public void SetActualRank_Invalid(int rank)
    {
        // arrange
        var country = new Country(5, "norge");

        // act & assert
        Assert.Throws<ArgumentException>(() => country.SetActualRank(rank));
    }
}
