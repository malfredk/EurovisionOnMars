using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

public class PlayerMapperTest
{
    private readonly PlayerMapper _mapper = new PlayerMapper();

    [Fact]
    public void UpdateEntity()
    {
        // arrange
        var originalEntity = new Player("malene");
        var dto = new PlayerDto(12, "bob");

        // act
        var updatedEntity = _mapper.UpdateEntity(originalEntity, dto);

        // assert
        Assert.Equal(updatedEntity.Username, originalEntity.Username);
        Assert.Equal(updatedEntity.Id, originalEntity.Id);
        Assert.NotEqual(updatedEntity.Username, dto.Username);
        Assert.NotEqual(updatedEntity.Id, dto.Id);
    }

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = new Player("malene");

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(dto.Username, entity.Username);
        Assert.Equal(dto.Id, entity.Id);
    }
}
