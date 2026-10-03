using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Api.Test.TestData.Players;

namespace EurovisionOnMars.Api.Test.Features.Players.GetPlayer;

public class PlayerMapperTest
{    
    private readonly PlayerMapper _mapper = new PlayerMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var playerEntity = PlayerFactory.CreateInitialPlayer();

        // act
        var playerDto = _mapper.ToDto(playerEntity);

        // assert
        Assert.Equal(PlayerTestData.Username, playerDto.Username);
        Assert.Equal(PlayerTestData.Id, playerDto.Id);
    }
}