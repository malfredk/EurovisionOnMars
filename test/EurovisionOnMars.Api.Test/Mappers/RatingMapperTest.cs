using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

public class RatingMapperTest
{
    private readonly RatingMapper _mapper = new RatingMapper();

    [Fact]
    public void UpdateEntity()
    {
        // arrange
        var playerEntity = new Player { Username = "jadda" };
        var originalEntity = new Rating
        {
            Id = 34,
            Category1 = 1,
            Category2 = null,
            Category3 = 3,
            PlayerId = 788888,
            Player = playerEntity
        };

        var dto = new RatingDto(787878, 100, 200, 300, 34);

        // act
        var updatedEntity = _mapper.UpdateEntity(originalEntity, dto);

        // assert
        Assert.Equal(updatedEntity.Id, originalEntity.Id);
        Assert.NotEqual(updatedEntity.Id, dto.Id);

        Assert.Equal(updatedEntity.Category1, dto.Category1);
        Assert.Equal(updatedEntity.Category2, dto.Category2);
        Assert.Equal(updatedEntity.Category3, dto.Category3);

        Assert.Equal(updatedEntity.PlayerId, originalEntity.PlayerId);
        Assert.NotEqual(updatedEntity.PlayerId, dto.PlayerId);

        Assert.Equal(updatedEntity.Player, originalEntity.Player);
    }

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = new Player { Username = "jadda" };
        var entity = new Rating
        {
            Id = 34,
            Category1 = 1,
            Category2 = 67,
            Category3 = 3,
            PlayerId = 788888,
            Player = playerEntity
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(dto.Id, entity.Id);
        Assert.Equal(dto.Category1, entity.Category1);
        Assert.Equal(dto.Category2, entity.Category2);
        Assert.Equal(dto.Category3, entity.Category3);
        Assert.Equal(dto.PlayerId, entity.PlayerId);
    }
}
