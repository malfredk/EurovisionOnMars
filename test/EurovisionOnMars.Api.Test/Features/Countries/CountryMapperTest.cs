using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryMapperTest
{
    private readonly CountryMapper _mapper = new CountryMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CreateEntity();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(Utils.COUNTRY_ID, dto.Id);
        Assert.Equal(Utils.COUNTRY_NUMBER, dto.Number);
        Assert.Equal(Utils.COUNTRY_NAME, dto.Name);
        Assert.Equal(Utils.COUNTRY_RANK, dto.ActualRank);
    }

    private Country CreateEntity()
    {
        var country = Utils.CreateInitialCountry();
        country.SetActualRank(Utils.COUNTRY_RANK);
        return country;
    }
}