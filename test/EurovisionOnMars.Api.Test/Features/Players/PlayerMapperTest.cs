using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players;

public class PlayerMapperTest
{    
    private readonly PlayerMapper _mapper = new PlayerMapper();

    private static readonly int ID = 123;
    private static readonly string USERNAME = "malfred";

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = CreatePlayerEntity();

        // act
        var playerDto = _mapper.ToDto(playerEntity);

        // assert
        Assert.Equal(USERNAME, playerDto.Username);
        Assert.Equal(ID, playerDto.Id);
    }

    private static Player CreatePlayerEntity()
    {
        return new Player
        {
            Id = ID,
            Username = USERNAME,
        };
    }
}