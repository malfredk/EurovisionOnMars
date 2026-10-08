using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.GameResults;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Players.Predictions;
using EurovisionOnMars.Entity.Test.TestData.Game;

namespace EurovisionOnMars.Entity.Test.Players.GameResults;

public class RatingGameResultsCalculatorTest
{
    private static readonly CountryPosition FirstPlace = new(1);
    private static readonly CountryPosition SecondPlace = new(2);
    private static readonly CountryPosition ThirdPlace = new(3);

    private static readonly BonusPoints SecondPlaceBonusPoints = new(-18);
    private static readonly BonusPoints ZeroBonusPoints = new(0);

    [Fact]
    public void Calculate_ValidRatings_CalculatesRankDifferencesForAllRatings()
    {
        // arrange
        var game = GameScenarioFactory.CreateGameWithOnePlayerWithCalculatedAndActualRank(3);
        var ratings = game.Players.First().PlayerRatings;

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.All(
            ratings,
            rating => Assert.NotNull(
                rating.RatingGameResult.RankDifference));
    }

    [Fact]
    public void Calculate_ValidRatings_CalculatesBonusPointsForAllRatings()
    {
        // arrange
        var game = GameScenarioFactory.CreateGameWithOnePlayerWithCalculatedAndActualRank(3);
        var ratings = game.Players.First().PlayerRatings;

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.All(
            ratings,
            rating => Assert.NotNull(
                rating.RatingGameResult.BonusPoints));
    }

    [Fact]
    public void Calculate_NotUniqueCalculatedRankButUniquePredictedRank_AwardsBonusPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2); 

        var ratings = game.Players.First().PlayerRatings;
        var tiedRating = ratings[0];
        var correctRating = ratings[1];

        tiedRating.Prediction.SetCalculatedRank(FirstPlace);
        correctRating.Prediction.SetCalculatedRank(FirstPlace);

        tiedRating.Prediction.SetTieBreakDemotion(new TieBreakDemotion(0));
        correctRating.Prediction.SetTieBreakDemotion(new TieBreakDemotion(1));

        var countries = game.Countries;
        countries[0].SetActualRank(ThirdPlace);
        countries[1].SetActualRank(SecondPlace);

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.Equal(
            SecondPlaceBonusPoints,
            correctRating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void Calculate_CorrectCalculatedRankButWrongPredictedRank_ZeroBonusPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(1);

        var ratings = game.Players.First().PlayerRatings;
        var rating = ratings[0];

        rating.Prediction.SetCalculatedRank(FirstPlace);
        rating.Prediction.SetTieBreakDemotion(new TieBreakDemotion(1));

        var countries = game.Countries;
        countries[0].SetActualRank(FirstPlace);

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.Equal(
            ZeroBonusPoints,
            rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void Calculate_NotUniquePredictedRank_ZeroBonusPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);

        var ratings = game.Players.First().PlayerRatings;
        var rating = ratings[0];
        var otherRating = ratings[1];

        rating.Prediction.SetCalculatedRank(FirstPlace);
        otherRating.Prediction.SetCalculatedRank(SecondPlace);

        var countries = game.Countries;
        countries[0].SetActualRank(SecondPlace);
        countries[1].SetActualRank(FirstPlace);

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.Equal(
            ZeroBonusPoints,
            rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void Calculate_MissingPredictedRank_ZeroBonusPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);

        var ratings = game.Players.First().PlayerRatings;
        var rating = ratings[0];
        var otherRating = ratings[1];

        otherRating.Prediction.SetCalculatedRank(SecondPlace);

        var countries = game.Countries;
        countries[0].SetActualRank(SecondPlace);
        countries[1].SetActualRank(FirstPlace);

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.Equal(
            ZeroBonusPoints,
            rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void Calculate_TiedPredictedRank_DoesNotAwardBonusPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);

        var ratings = game.Players.First().PlayerRatings;
        var rating = ratings[0];
        var tiedRating = ratings[1];

        rating.Prediction.SetCalculatedRank(SecondPlace);
        tiedRating.Prediction.SetCalculatedRank(SecondPlace);

        var countries = game.Countries;
        countries[0].SetActualRank(SecondPlace);
        countries[1].SetActualRank(SecondPlace);

        // act
        RatingGameResultsCalculator.Calculate(ratings);

        // assert
        Assert.Equal(
            ZeroBonusPoints,
            rating.RatingGameResult.BonusPoints);
        Assert.Equal(
            ZeroBonusPoints,
            tiedRating.RatingGameResult.BonusPoints);
    }
}