using EurovisionOnMars.Domain.Game;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.TestSupport.Players;

public class PlayerRanksCalculatorTest
{
    [Fact]
    public void CalculatePlayerRanks_DifferentPoints_RanksPlayersByAscendingPoints()
    {
        // arrange
        var player30 = PlayerFactory.CreateWithTotalGamePoints(30);
        var player10 = PlayerFactory.CreateWithTotalGamePoints(10);
        var player20 = PlayerFactory.CreateWithTotalGamePoints(20);

        var players = new[]
        {
            player30,
            player10,
            player20
        };

        // act
        PlayerRanksCalculator.Calculate(players);

        // assert
        Assert.Equal(1, player10.PlayerGameResult.Rank!.Value);
        Assert.Equal(2, player20.PlayerGameResult.Rank!.Value);
        Assert.Equal(3, player30.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_EqualPoints_GivesSameRank()
    {
        // arrange
        var player1 = PlayerFactory.CreateWithTotalGamePoints(10);
        var player2 = PlayerFactory.CreateWithTotalGamePoints(10);

        Player[] players = [player1, player2];

        // act
        PlayerRanksCalculator.Calculate(players);

        // assert
        Assert.Equal(1, player1.PlayerGameResult.Rank!.Value);
        Assert.Equal(1, player2.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_WorseThanTie_SkipsRank()
    {
        // arrange
        var player1 = PlayerFactory.CreateWithTotalGamePoints(20);
        var player2 = PlayerFactory.CreateWithTotalGamePoints(10);
        var player3 = PlayerFactory.CreateWithTotalGamePoints(10);

        Player[] players = [player1, player2, player3];

        // act
        PlayerRanksCalculator.Calculate(players);

        // assert
        Assert.Equal(3, player1.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_MultipleTies_AssignsCompetitionRanks()
    {
        // arrange
        var player1 = PlayerFactory.CreateWithTotalGamePoints(10);
        var player2 = PlayerFactory.CreateWithTotalGamePoints(10);
        var player3 = PlayerFactory.CreateWithTotalGamePoints(20);
        var player4 = PlayerFactory.CreateWithTotalGamePoints(20);
        var player5 = PlayerFactory.CreateWithTotalGamePoints(30);

        Player[] players =
        [
            player1,
            player2,
            player3,
            player4,
            player5
        ];

        // act
        PlayerRanksCalculator.Calculate(players);

        // assert
        Assert.Equal(1, player1.PlayerGameResult.Rank!.Value);
        Assert.Equal(1, player2.PlayerGameResult.Rank!.Value);

        Assert.Equal(3, player3.PlayerGameResult.Rank!.Value);
        Assert.Equal(3, player4.PlayerGameResult.Rank!.Value);

        Assert.Equal(5, player5.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_OnePlayer_AssignsRankOne()
    {
        // arrange
        var player = PlayerFactory.CreateWithTotalGamePoints(10);

        // act
        PlayerRanksCalculator.Calculate([player]);

        // assert
        Assert.Equal(1, player.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_NoPlayers_DoesNotThrow()
    {
        // arrange
        Player[] players = [];

        // act
        var action = () => PlayerRanksCalculator.Calculate(players);

        // assert
        var exception = Record.Exception(action);
        Assert.Null(exception);
    }
}