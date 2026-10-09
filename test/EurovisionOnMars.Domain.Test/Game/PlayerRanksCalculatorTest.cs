using EurovisionOnMars.Domain.Game;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Test.TestData.Players;

public class PlayerRanksCalculatorTest
{
    private readonly PlayerRanksCalculator _calculator = new();

    [Fact]
    public void CalculatePlayerRanks_DifferentPoints_RanksPlayersByAscendingPoints()
    {
        // arrange
        var player30 = PlayerFactory.CreatePlayerWithTotalPoints(30);
        var player10 = PlayerFactory.CreatePlayerWithTotalPoints(10);
        var player20 = PlayerFactory.CreatePlayerWithTotalPoints(20);

        var players = new[]
        {
            player30,
            player10,
            player20
        };

        // act
        _calculator.CalculatePlayerRanks(players);

        // assert
        Assert.Equal(1, player10.PlayerGameResult.Rank!.Value);
        Assert.Equal(2, player20.PlayerGameResult.Rank!.Value);
        Assert.Equal(3, player30.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_EqualPoints_GivesSameRank()
    {
        // arrange
        var player1 = PlayerFactory.CreatePlayerWithTotalPoints(10);
        var player2 = PlayerFactory.CreatePlayerWithTotalPoints(10);
        var player3 = PlayerFactory.CreatePlayerWithTotalPoints(20);

        Player[] players = [player1, player2, player3];

        // act
        _calculator.CalculatePlayerRanks(players);

        // assert
        Assert.Equal(1, player1.PlayerGameResult.Rank!.Value);
        Assert.Equal(1, player2.PlayerGameResult.Rank!.Value);
        Assert.Equal(3, player3.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_MultipleTies_AssignsCompetitionRanks()
    {
        // arrange
        var player1 = PlayerFactory.CreatePlayerWithTotalPoints(10);
        var player2 = PlayerFactory.CreatePlayerWithTotalPoints(10);
        var player3 = PlayerFactory.CreatePlayerWithTotalPoints(20);
        var player4 = PlayerFactory.CreatePlayerWithTotalPoints(20);
        var player5 = PlayerFactory.CreatePlayerWithTotalPoints(30);

        Player[] players =
        [
            player1,
            player2,
            player3,
            player4,
            player5
        ];

        // act
        _calculator.CalculatePlayerRanks(players);

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
        var player = PlayerFactory.CreatePlayerWithTotalPoints(10);

        // act
        _calculator.CalculatePlayerRanks([player]);

        // assert
        Assert.Equal(1, player.PlayerGameResult.Rank!.Value);
    }

    [Fact]
    public void CalculatePlayerRanks_NoPlayers_DoesNotThrow()
    {
        // arrange
        Player[] players = [];

        // act
        var action = () => _calculator.CalculatePlayerRanks(players);

        // assert
        var exception = Record.Exception(action);
        Assert.Null(exception);
    }
}