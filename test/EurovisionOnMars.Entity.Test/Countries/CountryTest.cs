using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Entity.Test.Countries;

public class CountryTest
{    
    [Fact]
    public void SetActualRank_Valid()
    {
        // arrange
        var country = Utils.CreateInitialCountry();
        var rank = new CountryPosition(19);

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }
}
