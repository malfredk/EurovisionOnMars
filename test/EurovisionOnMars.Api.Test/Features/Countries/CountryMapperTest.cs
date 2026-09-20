using EurovisionOnMars.Api.Features.Countries;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryMapperTest
{
    private readonly CountryMapper _mapper = new CountryMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = Utils.CreateRankedCountry();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(Utils.COUNTRY_ID, dto.Id);
        Assert.Equal(Utils.COUNTRY_NUMBER.Value, dto.Number);
        Assert.Equal(Utils.COUNTRY_NAME.Value, dto.Name);
        Assert.Equal(Utils.COUNTRY_RANK.Value, dto.ActualRank);
    }
}