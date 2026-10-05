using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Test.TestData.Game;
using EurovisionOnMars.Entity.Test.TestData.Players;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test.Players.Predictions;

public class PlayerTest
{
    private static readonly Username Username = new(PlayerTestData.Username);

    [Fact]
    public void Player_ValidInput_CreatesPlayer() {         
        // arrange
        var countries = CountryFactory.CreateInitialListWith2Countries();

        // act
        var player = new Player(Username, countries);

        // assert
        Assert.Equal(Username, player.Username);

        Assert.Equal(2, player.PlayerRatings.Count);
        Assert.Equal(countries, player.PlayerRatings.Select(pr => pr.Country));
        Assert.Same(player, player.PlayerRatings[0].Player);
        Assert.Same(player, player.PlayerRatings[1].Player);

        Assert.NotNull(player.PlayerGameResult);
        Assert.Same(player, player.PlayerGameResult.Player);
    }

    [Theory]
    [MemberData(nameof(NoCountriesTestData))]
    public void Player_NoCountries_ThrowException(ImmutableList<Country> countries)
    {
        // act and assert
        Assert.Throws<InvalidOperationException>(() => new Player(Username, countries));
    }

    public static IEnumerable<object[]> NoCountriesTestData =>
    new List<object[]>
    {
        new object[] { null! },
        new object[] { ImmutableList<Country>.Empty }
    };

    [Fact]
    public void CalculateGamePoints_CalculatesAllRankDifferences()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.All(
            player.PlayerRatings,
            rating =>
            {
                Assert.NotNull(
                    rating.RatingGameResult.RankDifference);
            });
    }

    [Fact]
    public void CalculateGamePoints_CalculatesAllBonusPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.All(
            player.PlayerRatings,
            rating =>
            {
                Assert.NotNull(
                    rating.RatingGameResult.BonusPoints);
            });

        Assert.NotNull(
            player.PlayerGameResult.TotalPoints);

        var expectedTotalPoints =
            player.PlayerRatings.Sum(rating =>
                rating.RatingGameResult.BonusPoints!.Value +
                Math.Abs(
                    rating.RatingGameResult.RankDifference!.Value));

        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void CalculateGamePoints_NotUniqueRank_NoBonusPoints() // TODO
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.All(
            player.PlayerRatings,
            rating =>
            {
                Assert.NotNull(
                    rating.RatingGameResult.BonusPoints);
            });

        Assert.NotNull(
            player.PlayerGameResult.TotalPoints);

        var expectedTotalPoints =
            player.PlayerRatings.Sum(rating =>
                rating.RatingGameResult.BonusPoints!.Value +
                Math.Abs(
                    rating.RatingGameResult.RankDifference!.Value));

        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void CalculateGamePoints_UniqueRank_BonusPoints() // TODO
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.All(
            player.PlayerRatings,
            rating =>
            {
                Assert.NotNull(
                    rating.RatingGameResult.BonusPoints);
            });

        Assert.NotNull(
            player.PlayerGameResult.TotalPoints);

        var expectedTotalPoints =
            player.PlayerRatings.Sum(rating =>
                rating.RatingGameResult.BonusPoints!.Value +
                Math.Abs(
                    rating.RatingGameResult.RankDifference!.Value));

        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void CalculateGamePoints_NegativeRankDifference_PositiveTotalPoints() // TODO
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.NotNull(
            player.PlayerGameResult.TotalPoints);

        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }


    [Fact]
    public void CalculateGamePoints_PositiveRankDifference_PositiveTotalPoints() // TODO
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.NotNull(
            player.PlayerGameResult.TotalPoints);

        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void CalculateGamePoints_MultipleRatingResults_SumsTotalPoints() // TODO
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOneInactivePlayerAndRankedCountries();
        var player = game.Players.First();

        // act
        player.CalculateGamePoints();

        // assert
        Assert.NotNull(
            player.PlayerGameResult.TotalPoints);

        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void RateCountry_ValidRating_SetsPoints()
    {
        // arrange
        var player = CreatePlayer();

        var rating = player.PlayerRatings[0];
        rating.Id = 1;

        var category1Points = new Points(4);
        var category2Points = new Points(6);
        var category3Points = new Points(8);

        // act
        player.RateCountry(
            rating.Id,
            category1Points,
            category2Points,
            category3Points);

        // assert
        Assert.Equal(category1Points, rating.Category1Points);
        Assert.Equal(category2Points, rating.Category2Points);
        Assert.Equal(category3Points, rating.Category3Points);
    }

    [Fact]
    public void RateCountry_RatingDoesNotExist_ThrowsKeyNotFoundException()
    {
        // arrange
        var player = CreatePlayer();

        const int invalidRatingId = 999;

        // act
        var action = () => player.RateCountry(
            invalidRatingId,
            new Points(4),
            new Points(6),
            new Points(8));

        // assert
        Assert.Throws<KeyNotFoundException>(action);
    }

    [Fact]
    public void RateCountry_ValidRating_RecalculatesPrediction()
    {
        // arrange
        var player = CreatePlayer();

        SetRatingIds(player);

        var rating = player.PlayerRatings[0];

        // act
        player.RateCountry(
            rating.Id,
            new Points(4),
            new Points(6),
            new Points(8));

        // assert
        Assert.Equal(
            18,
            rating.Prediction.TotalGivenPoints);

        Assert.NotNull(
            rating.Prediction.CalculatedRank);
    }

    [Fact]
    public void ResolveTieBreak_ValidTie_SetsTieBreakDemotionsInRequestedOrder()
    {
        // arrange
        var player = CreatePlayer();
        SetRatingIds(player);

        var rating1 = player.PlayerRatings[0];
        var rating2 = player.PlayerRatings[1];

        rating1.Prediction.Id = 1;
        rating2.Prediction.Id = 2;

        // Same total points -> same calculated rank
        player.RateCountry(
            rating1.Id,
            new Points(4),
            new Points(4),
            new Points(4));

        player.RateCountry(
            rating2.Id,
            new Points(4),
            new Points(4),
            new Points(4));

        List<int> orderedPredictionIds =
        [
            rating2.Prediction.Id,
            rating1.Prediction.Id
        ];

        // act
        player.ResolveTieBreak(orderedPredictionIds);

        // assert
        Assert.Equal(
            0,
            rating2.Prediction.TieBreakDemotion!.Value);

        Assert.Equal(
            1,
            rating1.Prediction.TieBreakDemotion!.Value);
    }

    private static Player CreatePlayer()
    {
        return new Player(
            new Username("lars"),
            CreateCountries());
    }

    private static void SetRatingIds(Player player)
    {
        for (var i = 0; i < player.PlayerRatings.Count; i++)
        {
            player.PlayerRatings[i].Id = i + 1;
        }
    }
}
