using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.GameResults;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test.Players.GameResults;

public class RatingGameResultsCalculatorTest
{
    [Fact]
    public void Calculate_ValidRatings_CalculatesRankDifferencesForAllRatings()
    {
        // arrange
        var player = CreatePlayerWithRankedCountries();

        var rating1 = player.PlayerRatings[0];
        var rating2 = player.PlayerRatings[1];
        var rating3 = player.PlayerRatings[2];

        rating1.Prediction.SetCalculatedRank(new CountryPosition(1));
        rating2.Prediction.SetCalculatedRank(new CountryPosition(3));
        rating3.Prediction.SetCalculatedRank(new CountryPosition(2));

        // actual ranks are 1, 2, 3

        // act
        RatingGameResultsCalculator.Calculate(player.PlayerRatings);

        // assert
        Assert.Equal(0, rating1.RatingGameResult.RankDifference);
        Assert.Equal(-1, rating2.RatingGameResult.RankDifference);
        Assert.Equal(1, rating3.RatingGameResult.RankDifference);
    }

    [Fact]
    public void Calculate_UniqueExactPrediction_AwardsBonusPoints()
    {
        // arrange
        var player = CreatePlayerWithRankedCountries();

        var rating1 = player.PlayerRatings[0];
        var rating2 = player.PlayerRatings[1];
        var rating3 = player.PlayerRatings[2];

        rating1.Prediction.SetCalculatedRank(new CountryPosition(1));
        rating2.Prediction.SetCalculatedRank(new CountryPosition(2));
        rating3.Prediction.SetCalculatedRank(new CountryPosition(3));

        // act
        RatingGameResultsCalculator.Calculate(player.PlayerRatings);

        // assert
        Assert.Equal(
            BonusPoints.FromRank(new CountryPosition(1)),
            rating1.RatingGameResult.BonusPoints);

        Assert.Equal(
            BonusPoints.FromRank(new CountryPosition(2)),
            rating2.RatingGameResult.BonusPoints);

        Assert.Equal(
            BonusPoints.FromRank(new CountryPosition(3)),
            rating3.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void Calculate_TiedPredictedRank_DoesNotAwardBonusPoints()
    {
        // arrange
        var player = CreatePlayerWithRankedCountries();

        var rating1 = player.PlayerRatings[0];
        var rating2 = player.PlayerRatings[1];
        var rating3 = player.PlayerRatings[2];

        rating1.Prediction.SetCalculatedRank(new CountryPosition(1));

        // Both predict rank 2, so rank 2 is not unique
        rating2.Prediction.SetCalculatedRank(new CountryPosition(2));
        rating3.Prediction.SetCalculatedRank(new CountryPosition(2));

        // act
        RatingGameResultsCalculator.Calculate(player.PlayerRatings);

        // assert
        Assert.Equal(
            new BonusPoints(0),
            rating2.RatingGameResult.BonusPoints);

        Assert.Equal(
            new BonusPoints(0),
            rating3.RatingGameResult.BonusPoints);
    }
}