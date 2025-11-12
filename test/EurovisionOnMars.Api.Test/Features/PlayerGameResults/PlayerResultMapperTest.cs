using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Features.PlayerGameResults;

public class PlayerResultMapperTest
{
    private static int PLAYER_GAME_RESULT_RANK = 89;
    private static int PLAYER_GAME_RESULT_ID = 92382;
    private static int PLAYER_GAME_RESULT_POINTS = 500;

    private readonly PlayerGameResultMapper _mapper = new PlayerGameResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CreateEntity();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(PLAYER_GAME_RESULT_ID, dto.Id);
        Assert.Equal(PLAYER_GAME_RESULT_RANK, dto.Rank);
        Assert.Equal(PLAYER_GAME_RESULT_POINTS, dto.TotalPoints);
        Assert.Equal(Utils.PLAYER_USERNAME, dto.PlayerUsername);
    }

    private static PlayerGameResult CreateEntity()
    {
        var player = Utils.CreateInitialPlayerWithOneCountry();
        var playerGameResult = player.PlayerGameResult;
        playerGameResult.Rank = PLAYER_GAME_RESULT_RANK;
        playerGameResult.TotalPoints = PLAYER_GAME_RESULT_POINTS;

        return playerGameResult;
    }
}
