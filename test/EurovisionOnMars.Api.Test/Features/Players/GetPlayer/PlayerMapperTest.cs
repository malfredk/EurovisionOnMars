using EurovisionOnMars.Api.Features.Players.GetPlayer;

namespace EurovisionOnMars.Api.Test.Features.Players.GetPlayer;

public class PlayerMapperTest
{    
    private readonly PlayerMapper _mapper = new PlayerMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = Utils.CreateInitialPlayer();

        // act
        var playerDto = _mapper.ToDto(playerEntity);

        // assert
        Assert.Equal(Utils.PLAYER_USERNAME.Value, playerDto.Username);
        Assert.Equal(Utils.PLAYER_ID, playerDto.Id);
    }
}