namespace EurovisionOnMars.Entity.Test;

public class CountryTest
{
    private readonly static CountryPosition Number = CountryPosition.Create(2);

    [Theory]
    [InlineData("østerrike")]
    [InlineData("san marino")]
    [InlineData("bosnia-hercegovina")]
    public void Country_Valid(string name)
    {
        // acts
        var country = new Country(Number, name);

        // assert
        Assert.Equal(name, country.Name);
        Assert.Equal(Number, country.Number);
    }

    [Theory]
    [InlineData("Danmark")]
    [InlineData("england")]
    [InlineData("danmark2")]
    [InlineData("danmark_")]
    [InlineData("")]
    public void Country_Invalid(string name)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new Country(Number, name));
    }

    [Fact]
    public void SetActualRank_Valid()
    {
        // arrange
        var country = new Country(Number, "norge");
        var rank = CountryPosition.Create(19);

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }
}
