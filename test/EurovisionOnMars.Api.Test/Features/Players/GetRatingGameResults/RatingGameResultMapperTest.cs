using EurovisionOnMars.Api.Features.Players.GetRatingGameResults;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Api.Test.TestData.Players;

namespace EurovisionOnMars.Api.Test.Features.Players.GetRatingGameResults;

public class RatingGameResultMapperTest
{
    private readonly RatingGameResultMapper _mapper = new RatingGameResultMapper();

    [Fact]
    public void ToDto_WithoutResults_ResturnsMostlyEmptyDto()
    {
        // arrange
        var playerRating = PlayerFactory.CreateInitialPlayer().PlayerRatings[0];

        // act
        var actual = _mapper.ToDto(playerRating);

        // assert
        Assert.Null(actual.RankDifference);
        Assert.Null(actual.BonusPoints);

        var countryDto = actual.Country;
        Assert.Null(countryDto.ActualRank);
        Assert.Equal(CountryTestData.Name, countryDto.Name);
    }

    [Fact]
    public void ToDto_WithResults_ResturnsDto()
    {
        // arrange
        var playerRating = PlayerFactory.CreatePlayerAtEndOfGame().PlayerRatings[0];
        
        var ratingGameResult = playerRating.RatingGameResult;
        int expectedRankDifference = ratingGameResult.RankDifference!.Value;
        int expectedBonusPoints = ratingGameResult.BonusPoints!.Value;

        // act
        var actual = _mapper.ToDto(playerRating);

        // assert
        Assert.Equal(expectedRankDifference, actual.RankDifference);
        Assert.Equal(expectedBonusPoints, actual.BonusPoints);

        var countryDto = actual.Country;
        Assert.Equal(CountryTestData.Rank, countryDto.ActualRank);
        Assert.Equal(CountryTestData.Name, countryDto.Name);
    }
}
