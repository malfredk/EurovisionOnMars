using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using Moq;

namespace EurovisionOnMars.Api.Test.Mappers;

public class PlayerMapperTest
{    
    private readonly Mock<IRatingMapper> _ratingMapperMock;
    private readonly PlayerMapper _mapper;

    public PlayerMapperTest()
    {
        _ratingMapperMock = new Mock<IRatingMapper>();

        _mapper = new PlayerMapper(_ratingMapperMock.Object);
    }

    [Fact]
    public void UpdateEntity()
    {
        // arrange
        var originalPlayerEntity = CreatePlayerEntity(167, "malene");

        var originalRatingEntity1 = CreateRatingEntity(5666, originalPlayerEntity);
        var originalRatingEntity2 = CreateRatingEntity(28, originalPlayerEntity);

        var updatedRatingEntity1 = CreateRatingEntity(4, originalPlayerEntity);
        var updatedRatingEntity2 = CreateRatingEntity(3, originalPlayerEntity);

        originalPlayerEntity.Ratings = new List<Rating> { originalRatingEntity1, originalRatingEntity2 };

        var ratingDto1 = CreateRatingDto(5666);
        var ratingDto2 = CreateRatingDto(28);

        var playerDto = CreatePlayerDto(new List<RatingDto> { ratingDto1, ratingDto2 });

        _ratingMapperMock.Setup(m => m.UpdateEntity(originalRatingEntity1, ratingDto1))
            .Returns(updatedRatingEntity1);
        _ratingMapperMock.Setup(m => m.UpdateEntity(originalRatingEntity2, ratingDto2))
            .Returns(updatedRatingEntity2);

        // act
        var updatedPlayerEntity = _mapper.UpdateEntity(originalPlayerEntity, playerDto);

        // assert
        Assert.Equal(updatedPlayerEntity.Username, originalPlayerEntity.Username);
        Assert.NotEqual(updatedPlayerEntity.Username, playerDto.Username);
        
        Assert.Equal(updatedPlayerEntity.Id, originalPlayerEntity.Id);
        Assert.NotEqual(updatedPlayerEntity.Id, playerDto.Id);

        Assert.Equal(updatedPlayerEntity.Ratings[0], updatedRatingEntity1);
        Assert.Equal(updatedPlayerEntity.Ratings[1], updatedRatingEntity2);

        _ratingMapperMock.Verify(m => m.UpdateEntity(originalRatingEntity1, ratingDto1), Times.Once);
        _ratingMapperMock.Verify(m => m.UpdateEntity(originalRatingEntity2, ratingDto2), Times.Once);
    }

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = CreatePlayerEntity(6, "bob");

        var ratingEntity1 = CreateRatingEntity(5666, playerEntity);
        var ratingEntity2 = CreateRatingEntity(28, playerEntity);

        playerEntity.Ratings = new List<Rating> { ratingEntity1, ratingEntity2 };

        var ratingDto1 = CreateRatingDto(5666);
        var ratingDto2 = CreateRatingDto(28);

        _ratingMapperMock.Setup(m => m.ToDto(ratingEntity1))
            .Returns(ratingDto1);
        _ratingMapperMock.Setup(m => m.ToDto(ratingEntity2))
            .Returns(ratingDto2);

        // act
        var playerDto = _mapper.ToDto(playerEntity);

        // assert
        Assert.Equal(playerDto.Username, playerEntity.Username);
        Assert.Equal(playerDto.Id, playerEntity.Id);
        Assert.Equal(playerDto.Ratings[0], ratingDto1);
        Assert.Equal(playerDto.Ratings[1], ratingDto2);

        _ratingMapperMock.Verify(m => m.ToDto(ratingEntity1), Times.Once);
        _ratingMapperMock.Verify(m => m.ToDto(ratingEntity2), Times.Once);
    }

    private static Player CreatePlayerEntity(int id, string username)
    {
        return new Player
        {
            Id = id,
            Username = username
        };
    }

    private static Rating CreateRatingEntity(int id, Player playerEntity)
    {
        return new Rating
        {
            Id = id,
            Category1Points = 1,
            Category2Points = null,
            Category3Points = 3,
            PlayerId = 788888,
            Player = playerEntity
        };
    }

    private static PlayerDto CreatePlayerDto(List<RatingDto> ratings)
    {
        return new PlayerDto
        {
            Id = 345,
            Username = "test",
            Ratings = ratings
        };
    }

    private static RatingDto CreateRatingDto(int id)
    {
        return new RatingDto
        {
            Id = id,
            Category1Points = 100,
            Category2Points = 200,
            Category3Points = 300,
            PlayerId = 34
        };
    }
}
