using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
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
        Assert.Equal(dto.Id, entity.Id);
        Assert.Equal(dto.Number, entity.Number);
        Assert.Equal(dto.Name, entity.Name);
        Assert.Equal(dto.Ranking, entity.Ranking);
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
