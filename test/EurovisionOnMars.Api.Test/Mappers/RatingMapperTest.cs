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
            Category1Points = 1,
            Category2Points = null,
            Category3Points = 3,
            PlayerId = 788888,
            Player = playerEntity
        };

        var dto = new RatingDto
        {
            Id = 787878,
            Category1Points = 100,
            Category2Points = 200,
            Category3Points = 300,
            PlayerId = 34
        };

        // act
        var updatedEntity = _mapper.UpdateEntity(originalEntity, dto);

        // assert
        Assert.Equal(updatedEntity.Id, originalEntity.Id);
        Assert.NotEqual(updatedEntity.Id, dto.Id);

        Assert.Equal(updatedEntity.Category1Points, dto.Category1Points);
        Assert.Equal(updatedEntity.Category2Points, dto.Category2Points);
        Assert.Equal(updatedEntity.Category3Points, dto.Category3Points);

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
            Category1Points = 1,
            Category2Points = 67,
            Category3Points = 3,
            PlayerId = 788888,
            Player = playerEntity
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(dto.Id, entity.Id);
        Assert.Equal(dto.Category1Points, entity.Category1Points);
        Assert.Equal(dto.Category2Points, entity.Category2Points);
        Assert.Equal(dto.Category3Points, entity.Category3Points);
        Assert.Equal(dto.PlayerId, entity.PlayerId);
    }
}
