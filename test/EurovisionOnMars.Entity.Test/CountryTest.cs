namespace EurovisionOnMars.Entity.Test;

public class CountryTest
{
    private readonly static CountryPosition Number = CountryPosition.Create(2);
    private readonly static CountryName Name = CountryName.Create("norge");
    
    [Fact]
    public void SetActualRank_Valid()
    {
        // arrange
        var country = new Country(Number, Name);
        var rank = CountryPosition.Create(19);

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }
}
