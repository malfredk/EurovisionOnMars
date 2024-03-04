using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

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
        Assert.Equal(entity.Ranking, dto.Ranking);
    }

    private Country CreateEntity()
    {
        return new Country
        {
            Id = 34,
            Number = 45,
            Name = "noreg",
            Ranking = 6383
        };
    }
}