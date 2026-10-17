using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.GameResults;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Players.Predictions;
using EurovisionOnMars.TestSupport.Game;
using EurovisionOnMars.TestSupport.Players;

namespace EurovisionOnMars.Domain.Test.Players.GameResults;

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
        var player = PlayerFactory.CreateWithRankedCountries(ratingCount: 3);
        var ratings = player.PlayerRatings;

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
        var player = PlayerFactory.CreateWithRankedCountries(ratingCount: 3);
        var ratings = player.PlayerRatings;

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
        var player = PlayerFactory.CreateInitial(ratingCount: 2); 
        var ratings = player.PlayerRatings;

        var tiedRating = ratings[0];
        var correctRating = ratings[1];

        tiedRating.Prediction.SetCalculatedRank(FirstPlace);
        correctRating.Prediction.SetCalculatedRank(FirstPlace);

        tiedRating.Prediction.SetTieBreakDemotion(new TieBreakDemotion(0));
        correctRating.Prediction.SetTieBreakDemotion(new TieBreakDemotion(1));

        tiedRating.Country!.SetActualRank(ThirdPlace);
        correctRating.Country!.SetActualRank(SecondPlace);

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
        var player = PlayerFactory.CreateInitialWithOneRating();
        var ratings = player.PlayerRatings;
        var rating = ratings[0];

        rating.Prediction.SetCalculatedRank(FirstPlace);
        rating.Prediction.SetTieBreakDemotion(new TieBreakDemotion(1));

        rating.Country!.SetActualRank(FirstPlace);

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
        var player = PlayerFactory.CreateInitial(ratingCount: 2);
        var ratings = player.PlayerRatings;

        var rating = ratings[0];
        var otherRating = ratings[1];

        rating.Prediction.SetCalculatedRank(FirstPlace);
        otherRating.Prediction.SetCalculatedRank(SecondPlace);

        rating.Country!.SetActualRank(SecondPlace);
        otherRating.Country!.SetActualRank(FirstPlace);

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
        var player = PlayerFactory.CreateInitial(ratingCount: 2);
        var ratings = player.PlayerRatings;

        var rating = ratings[0];
        var otherRating = ratings[1];

        otherRating.Prediction.SetCalculatedRank(SecondPlace);

        rating.Country!.SetActualRank(SecondPlace);
        otherRating.Country!.SetActualRank(FirstPlace);

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
        var player = PlayerFactory.CreateInitial(ratingCount: 2);
        var ratings = player.PlayerRatings;

        var rating = ratings[0];
        var tiedRating = ratings[1];

        rating.Prediction.SetCalculatedRank(SecondPlace);
        tiedRating.Prediction.SetCalculatedRank(SecondPlace);

        rating.Country!.SetActualRank(SecondPlace);
        tiedRating.Country!.SetActualRank(SecondPlace);

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