using EurovisionOnMars.Api.Features.Players.GetRatingGameResults;
using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.TestSupport.Countries;
using EurovisionOnMars.TestSupport.Players;
using static MudBlazor.CategoryTypes;

namespace EurovisionOnMars.Api.Test.Features.Players.GetRatingGameResults;

public class RatingGameResultMapperTest
{
    private readonly RatingGameResultMapper _mapper = new RatingGameResultMapper();

    [Fact]
    public void ToDto_WithoutResults_ResturnsMostlyEmptyDto()
    {
        // arrange
        var entity = PlayerFactory.CreateInitialWithOneRating().PlayerRatings[0];

        // act
        var actual = _mapper.ToDto(entity);

        // assert
        Assert.Null(actual.RankDifference);
        Assert.Null(actual.BonusPoints);

        var countryDto = actual.Country;
        Assert.Null(countryDto.ActualRank);
        Assert.Equal(CountryTestData.Name, countryDto.Name);
    }

    [Fact]
    public void ToDto_WithResults_ResturnsCompleteDto()
    {
        // arrange
        var bonusPoints = -2;
        var rankDifference = 5;

        var ratingEntity = PlayerFactory
            .CreateWithCompleteRatingGameResult(bonusPoints, rankDifference)
            .PlayerRatings[0];

        ratingEntity.Country!.SetActualRank(new CountryPosition(CountryTestData.Rank));

        // act
        var actual = _mapper.ToDto(ratingEntity);

        // assert
        Assert.Equal(rankDifference, actual.RankDifference);
        Assert.Equal(bonusPoints, actual.BonusPoints);

        var countryDto = actual.Country;
        Assert.Equal(CountryTestData.Rank, countryDto.ActualRank);
        Assert.Equal(CountryTestData.Name, countryDto.Name);
    }
}
