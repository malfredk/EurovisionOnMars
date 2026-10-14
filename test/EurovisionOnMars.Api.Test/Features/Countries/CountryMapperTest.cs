using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.TestSupport.Countries;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryMapperTest
{
    private readonly CountryMapper _mapper = new CountryMapper();

    [Fact]
    public void ToDto_WithRank()
    {
        // arrange
        var entity = CreateCountryWithRank();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(CountryTestData.Number, dto.Number);
        Assert.Equal(CountryTestData.Name, dto.Name);
        Assert.Equal(CountryTestData.Rank, dto.ActualRank);
    }

    [Fact]
    public void ToDto_WithoutRank()
    {
        // arrange
        var entity = CountryFactory.CreateInitial();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(CountryTestData.Number, dto.Number);
        Assert.Equal(CountryTestData.Name, dto.Name);
        Assert.Null(dto.ActualRank);
    }

    private static Country CreateCountryWithRank()
    {
        var country = CountryFactory.CreateInitial();
        country.SetActualRank(new CountryPosition(CountryTestData.Rank));
        return country;
    }
}