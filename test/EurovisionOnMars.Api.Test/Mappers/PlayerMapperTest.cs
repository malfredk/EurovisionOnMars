using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using Moq;

namespace EurovisionOnMars.Api.Test.Mappers;

public class PlayerMapperTest
{    
    private readonly Mock<IRatingMapper> _ratingMapperMock;
    private readonly Mock<IPlayerResultMapper> _playerResultMapperMock;
    private readonly PlayerMapper _mapper;

    public PlayerMapperTest()
    {
        _ratingMapperMock = new Mock<IRatingMapper>();
        _playerResultMapperMock = new Mock<IPlayerResultMapper>();

        _mapper = new PlayerMapper(_ratingMapperMock.Object, _playerResultMapperMock.Object);
    }

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = CreatePlayerEntity(6, "bob");

        var ratingEntity1 = CreateRatingEntity(5666, playerEntity);
        var ratingEntity2 = CreateRatingEntity(28, playerEntity);

        var resultEntity = new PlayerResult { Id = 12738 };

        playerEntity.Ratings = new List<Rating> { ratingEntity1, ratingEntity2 };
        playerEntity.PlayerResult = resultEntity;

        var ratingDto1 = CreateRatingDto(5666);
        var ratingDto2 = CreateRatingDto(28);

        var resultDto = new PlayerResultDto { Id = 673 };

        _ratingMapperMock.Setup(m => m.ToDto(ratingEntity1))
            .Returns(ratingDto1);
        _ratingMapperMock.Setup(m => m.ToDto(ratingEntity2))
            .Returns(ratingDto2);

        _playerResultMapperMock.Setup(m => m.ToDto(resultEntity))
            .Returns(resultDto);

        // act
        var playerDto = _mapper.ToDto(playerEntity);

        // assert
        Assert.Equal(playerEntity.Username, playerDto.Username);
        Assert.Equal(playerEntity.Id, playerDto.Id);
        Assert.Equal(ratingDto1, playerDto.Ratings[0]);
        Assert.Equal(ratingDto2, playerDto.Ratings[1]);
        Assert.Equal(resultDto, playerDto.PlayerResult);

        _ratingMapperMock.Verify(m => m.ToDto(ratingEntity1), Times.Once);
        _ratingMapperMock.Verify(m => m.ToDto(ratingEntity2), Times.Once);
        _playerResultMapperMock.Verify(m => m.ToDto(resultEntity), Times.Once);
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
            Player = playerEntity,
            CountryId = 89
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
            PlayerId = 34,
            CountryId = 88
        };
    }
}