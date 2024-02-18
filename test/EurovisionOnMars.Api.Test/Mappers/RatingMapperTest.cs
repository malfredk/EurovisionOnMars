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
        var originalEntity = CreateRatingEntity();

        var dto = new RatingDto
        {
            Id = 787878,
            Category1Points = 100,
            Category2Points = 200,
            Category3Points = 300,
            PlayerId = 34,
            PointsSum = 677,
            Ranking = 13
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

        Assert.Equal(updatedEntity.PointsSum, 600);
        Assert.NotEqual(updatedEntity.PointsSum, dto.PointsSum);

        Assert.Equal(updatedEntity.Ranking, dto.Ranking);
    }

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CreateRatingEntity();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(dto.Id, entity.Id);
        Assert.Equal(dto.Category1Points, entity.Category1Points);
        Assert.Equal(dto.Category2Points, entity.Category2Points);
        Assert.Equal(dto.Category3Points, entity.Category3Points);
        Assert.Equal(dto.PlayerId, entity.PlayerId);
        Assert.Equal(dto.PointsSum, entity.PointsSum);
        Assert.Equal(dto.Ranking, entity.Ranking);
    }

    private Rating CreateRatingEntity()
    {
        var playerEntity = new Player { Username = "jadda" };
        return new Rating
        {
            Id = 34,
            Category1Points = 1,
            Category2Points = null,
            Category3Points = 3,
            PlayerId = 788888,
            Player = playerEntity,
            PointsSum = 9000,
            Ranking = 26
        };
    }
}
