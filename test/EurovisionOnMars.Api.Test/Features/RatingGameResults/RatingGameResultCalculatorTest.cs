using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Features.RatingGameResults;

public class RatingGameResultCalculatorTest
{
    private readonly RatingGameResultCalculator _calculator = new RatingGameResultCalculator();

    // tests for CalculateRatingGameResult

    [Fact]
    public void CalculateRatingGameResult()
    {
        // arrange
        var rating = CreatePlayerRating(10, 7);

        var otherRating = CreatePlayerRating(1, 1);

        var ratings = new List<PlayerRating>
        {
            rating,
            otherRating
        };

        // act
        _calculator.CalculateRatingGameResult(rating, ratings);

        // assert
        Assert.Equal(-3, rating.RatingGameResult.RankDifference);
        Assert.Equal(0, rating.RatingGameResult.BonusPoints);

        Assert.Null(otherRating.RatingGameResult.RankDifference);
        Assert.Null(otherRating.RatingGameResult.BonusPoints);
    }

    // tests for CalculateRankDifference

    [Theory]
    [InlineData(1, 8)]
    [InlineData(11, 0)]
    [InlineData(20, -11)]
    public void CalculateRankDifference(int calculatedRank, int expectedRankDifference)
    {
        // arrange
        int actualRank = 10;
        var playerRating = CreatePlayerRating(actualRank, calculatedRank, 1);

        // act
        _calculator.CalculateRankDifference(playerRating);

        // assert
        Assert.Equal(expectedRankDifference, playerRating.RatingGameResult.RankDifference);
    }

    [Fact]
    public void CalculateRankDifference_NoCalculatedRank()
    {
        // arrange
        int actualRank = 10;
        var playerRating = CreateUnrankedPlayerRating(actualRank);

        // act
        _calculator.CalculateRankDifference(playerRating);

        // assert
        Assert.Equal(26, playerRating.RatingGameResult.RankDifference);
    }

    // tests for CalculateBonusPoints

    [Theory]
    [InlineData(1, -25)]
    [InlineData(2, -18)]
    [InlineData(3, -15)]
    [InlineData(4, -12)]
    [InlineData(5, -10)]
    [InlineData(6, -8)]
    [InlineData(7, -6)]
    [InlineData(8, -4)]
    [InlineData(9, -2)]
    [InlineData(10, -1)]
    [InlineData(11, 0)]
    [InlineData(26, 0)]
    public void CalculateBonusPoints_ZeroRankDifferenceAndUniqueRank(int actualRank, int expectedBonusPoints)
    {
        // arrange
        var rating = CreatePlayerRatingWithRankDifference(actualRank, actualRank, 0);
        var ratingWithDifferentPredictedRank = CreatePlayerRatingWithRankDifference(4, 13, 0);
        var otherRatingWithoutCalculatedRank = CreateUnrankedPlayerRating(13);

        var ratings = new List<PlayerRating>
        {
            rating,
            ratingWithDifferentPredictedRank,
            otherRatingWithoutCalculatedRank,
        };

        // act
        _calculator.CalculateBonusPoints(rating, ratings);

        // assert
        Assert.Equal(expectedBonusPoints, rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void CalculateBonusPoints_ZeroRankDifferenceAndDuplicateRank()
    {
        // arrange
        var calculatedRank = 1;
        var correctRating = CreatePlayerRatingWithRankDifference(calculatedRank, calculatedRank, 0);
        var ratingWithSameRank = CreatePlayerRating(10, calculatedRank);

        var ratings = new List<PlayerRating>
        {
            correctRating,
            ratingWithSameRank
        };

        // act
        _calculator.CalculateBonusPoints(correctRating, ratings);

        // assert
        Assert.Equal(0, correctRating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void CalculateBonusPoints_NonZeroRankDifference()
    {
        // arrange
        var rating = CreatePlayerRatingWithRankDifference(1, 1, 23);
        var ratings = new List<PlayerRating>
        {
            rating
        };

        // act
        _calculator.CalculateBonusPoints(rating, ratings);

        // assert
        Assert.Equal(0, rating.RatingGameResult.BonusPoints);
    }

    private static PlayerRating CreateUnrankedPlayerRating(
        int actualRank
    )
    {
        var rating = Utils.CreateInitialPlayerRating();
        rating.Country!.SetActualRank(actualRank);
        return rating;
    }

    private static PlayerRating CreatePlayerRating(
        int actualRank, 
        int calculatedRank,
        int? tieBreakDemotion = null
    )
    {
        var rating = CreateUnrankedPlayerRating(actualRank);
        rating.Prediction.SetCalculatedRank(calculatedRank);
        rating.Prediction.SetTieBreakDemotion(tieBreakDemotion);
        return rating;
    }

    private static PlayerRating CreatePlayerRatingWithRankDifference(
        int actualRank,
        int calculatedRank,
        int rankDifference
    )
    {
        var rating = CreatePlayerRating(actualRank, calculatedRank);
        rating.RatingGameResult.RankDifference = rankDifference;
        return rating;
    }
}
