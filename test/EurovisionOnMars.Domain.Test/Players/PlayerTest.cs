using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.TestSupport.Countries;
using EurovisionOnMars.TestSupport.Game;
using EurovisionOnMars.TestSupport.Players;
using EurovisionOnMars.TestSupport.Players.PlayerRatings;
using System.Collections.Immutable;

namespace EurovisionOnMars.Domain.Test.Players;

public class PlayerTest
{
    private static readonly Username Username = new(PlayerTestData.Username);

    private const int RatingId = 77;

    private static readonly Points SpecialPoints = new(12);
    private static readonly Points Category1Points = new(PlayerRatingTestData.Category1Points);
    private static readonly Points Category2Points = new(PlayerRatingTestData.Category2Points);
    private static readonly Points Category3Points = new(PlayerRatingTestData.Category3Points);

    // tests for constructor, Player

    [Fact]
    public void Constructor_ValidInput_CreatesPlayerWithRatingsAndResult() {         
        // arrange
        var countries = CountryFactory.CreateInitialList(2);

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
    public void Constructor_NoCountries_ThrowsInvalidOperationException(ImmutableList<Country> countries)
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

    // tests for CalculateGamePoints

    [Fact]
    public void CalculateGamePoints_CalculatesAllGamePoints()
    {
        // arrange
        var player = PlayerFactory.CreateWithRankedCountries(3);

        // act
        player.CalculateGamePoints();

        // assert
        Assert.All(
            player.PlayerRatings,
            rating =>
            {
                Assert.NotNull(rating.RatingGameResult.RankDifference);
                Assert.NotNull(rating.RatingGameResult.BonusPoints);
            });

        Assert.NotNull(player.PlayerGameResult.TotalPoints);
    }

    // tests for RateCountry

    [Fact]
    public void RateCountry_InitialRating_SetsPoints()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var rating = player.PlayerRatings[0];
        rating.Id = RatingId;

        // act
        player.RateCountry(
            RatingId,
            Category1Points,
            Category2Points,
            Category3Points);

        // assert
        Assert.Equal(Category1Points, rating.Category1Points);
        Assert.Equal(Category2Points, rating.Category2Points);
        Assert.Equal(Category3Points, rating.Category3Points);
    }

    [Fact]
    public void RateCountry_InitialRating_CalculatesPrediction()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var rating = player.PlayerRatings[0];
        rating.Id = RatingId;

        // act
        player.RateCountry(
            RatingId,
            Category1Points,
            Category2Points,
            Category3Points);

        // assert
        Assert.NotNull(rating.Prediction.TotalGivenPoints);
        Assert.NotNull(rating.Prediction.CalculatedRank);
    }

    [Fact]
    public void RateCountry_UnknownRating_ThrowsKeyNotFoundException()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        player.PlayerRatings[0].Id = RatingId;

        const int unknownRatingId = 999;

        // act
        var action = () => player.RateCountry(
            unknownRatingId,
            Category1Points,
            Category2Points,
            Category3Points);

        // assert
        Assert.Throws<KeyNotFoundException>(action);
    }

    [Fact]
    public void RateCountry_SameSpecialPoints_ThrowsAndDoesNotRateCountry()
    {
        // arrange
        var player = PlayerFactory.CreateInitial(2);

        var rating = player.PlayerRatings[0];
        rating.Id = RatingId;

        var otherRating = player.PlayerRatings[1];
        otherRating.Id = 99;
        otherRating.SetPoints(SpecialPoints, Category2Points, Category3Points);

        // act
        var action = () => player.RateCountry(
            RatingId,
            SpecialPoints,
            Category2Points,
            Category3Points);

        // assert
        Assert.Throws<ArgumentException>(action);

        Assert.Null(rating.Category1Points);
        Assert.Null(rating.Category2Points);
        Assert.Null(rating.Category3Points);
    }

    // tests for ResolveTieBreak

    [Fact]
    public void ResolveTieBreak_ValidInput_SetsTieBreakDemotions()
    {
        // arrange
        var player = CreatePlayerWith2RatingsWithSameRank();

        var prediction1 = player.PlayerRatings[0].Prediction;
        var prediction2 = player.PlayerRatings[1].Prediction;

        var prediction1Id = 101;
        prediction1.Id = prediction1Id;

        var prediction2Id = 202;
        prediction2.Id = prediction2Id;

        List<int> orderedPredictionIds = [prediction1Id, prediction2Id];

        // act
        player.ResolveTieBreak(orderedPredictionIds);

        // assert
        Assert.NotNull(prediction1.TieBreakDemotion);
        Assert.NotNull(prediction2.TieBreakDemotion);
    }

    private static Player CreatePlayerWith2RatingsWithSameRank()
    {
        var player = PlayerFactory.CreateInitial(2);

        var tiedRank = new CountryPosition(1);

        player.PlayerRatings[0].Prediction.SetCalculatedRank(tiedRank);
        player.PlayerRatings[1].Prediction.SetCalculatedRank(tiedRank);

        return player;
    }
}
