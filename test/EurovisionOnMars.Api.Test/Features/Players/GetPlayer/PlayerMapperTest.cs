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
        var entity = PlayerFactory.CreateInitialPlayer();

        // act
        var actual = _mapper.ToDto(entity);

        // assert
        Assert.Equal(PlayerTestData.Username, actual.Username);
        Assert.Equal(entity.Id, actual.Id);
    }
}