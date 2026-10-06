using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Test.TestData.Players;

namespace EurovisionOnMars.Entity.Test.Players.GameResults;

public class PlayerGameResultTest
{
    [Fact]
    public void Constructor_CreatesResultForPlayer()
    {
        // act
        var player = PlayerFactory.CreateInitialPlayer();

        // assert
        var result = player.PlayerGameResult;
        Assert.Same(player, result.Player);
        Assert.Null(result.Rank);
        Assert.Null(result.TotalPoints);
    }

    [Fact]
    public void SetRank_SetsRank()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayer();
        var result = player.PlayerGameResult;
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
        var player = PlayerFactory.CreateInitialPlayer();
        var result = player.PlayerGameResult;
        const int totalPoints = 42;

        // act
        result.SetTotalPoints(totalPoints);

        // assert
        Assert.Equal(totalPoints, result.TotalPoints);
    }
}