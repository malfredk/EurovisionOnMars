using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Entity.Test.Countries;

public class CountryTest
{
    private readonly static CountryPosition Number = new CountryPosition(2);
    private readonly static CountryName Name = new CountryName("norge");
    
    [Fact]
    public void SetActualRank_Valid()
    {
        // arrange
        var country = new Country(Number, Name);
        var rank = new CountryPosition(19);

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }
}
