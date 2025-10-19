using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using Moq;

namespace EurovisionOnMars.Api.Test.Mappers;

public class PlayerMapperTest
{    
    private readonly Mock<IPlayerRatingMapper> _ratingMapperMock;
    private readonly Mock<IPlayerGameResultMapper> _playerResultMapperMock;
    private readonly PlayerMapper _mapper;

    public PlayerMapperTest()
    {
        _ratingMapperMock = new Mock<IPlayerRatingMapper>();
        _playerResultMapperMock = new Mock<IPlayerGameResultMapper>();

        _mapper = new PlayerMapper(_ratingMapperMock.Object, _playerResultMapperMock.Object);
    }

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = CreatePlayerEntity(6, "bob");

        var ratingEntity1 = CreateRatingEntity(5666, playerEntity);
        var ratingEntity2 = CreateRatingEntity(28, playerEntity);

        var resultEntity = new PlayerGameResult { Id = 12738 };

        playerEntity.PlayerRatings = new List<PlayerRating> { ratingEntity1, ratingEntity2 };
        playerEntity.PlayerGameResult = resultEntity;

        var ratingDto1 = CreateRatingDto(5666);
        var ratingDto2 = CreateRatingDto(28);

        var resultDto = new PlayerGameResultDto { Id = 673 };

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
        Assert.Equal(ratingDto1, playerDto.PlayerRatings[0]);
        Assert.Equal(ratingDto2, playerDto.PlayerRatings[1]);
        Assert.Equal(resultDto, playerDto.PlayerGameResult);

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

    private static PlayerRating CreateRatingEntity(int id, Player playerEntity)
    {
        return new PlayerRating
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

    private static PlayerRatingDto CreateRatingDto(int id)
    {
        return new PlayerRatingDto
        {
            Id = id,
            Category1Points = 100,
            Category2Points = 200,
            Category3Points = 300
        };
    }
}