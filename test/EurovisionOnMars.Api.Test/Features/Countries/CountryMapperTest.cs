using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Test.TestData.Countries;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryMapperTest
{
    private readonly CountryMapper _mapper = new CountryMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CountryFactory.CreateRankedCountry();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(CountryTestData.Id, dto.Id);
        Assert.Equal(CountryTestData.Number, dto.Number);
        Assert.Equal(CountryTestData.Name, dto.Name);
        Assert.Equal(CountryTestData.Rank, dto.ActualRank);
    }
}