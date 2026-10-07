using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.GameResults;
using EurovisionOnMars.Entity.Test.TestData.Players;

namespace EurovisionOnMars.Entity.Test.Players.GameResults;

public class PlayerGameResultTest
{
    [Fact]
    public void Constructor_CreatesResultForPlayer()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayer();

        // act
        var result = new PlayerGameResult(player);

        // assert
        Assert.Same(player, result.Player);
        Assert.Null(result.Rank);
        Assert.Null(result.TotalPoints);
    }

    [Fact]
    public void SetRank_SetsRank()
    {
        // arrange
        var result = PlayerFactory.CreateInitialPlayer().PlayerGameResult;
        var rank = new PlayerRank(3);

        // act
        result.SetRank(rank);

        // assert
        Assert.Equal(rank, result.Rank);
    }

    [Fact]
    public void SetTotalPoints_SetsTotalPoints()
    {
        // arrange
        var result = PlayerFactory.CreateInitialPlayer().PlayerGameResult;
        const int totalPoints = 42;

        // act
        result.SetTotalPoints(totalPoints);

        // assert
        Assert.Equal(totalPoints, result.TotalPoints);
    }
}