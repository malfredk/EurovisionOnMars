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
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Number, dto.Number);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.ActualRank, dto.ActualRank);
    }

    private Country CreateEntity()
    {
        return new Country
        {
            Id = 34,
            Number = 45,
            Name = "noreg",
            ActualRank = 6383
        };
    }
}