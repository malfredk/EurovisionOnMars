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
        var dto = _mapper.ToDto(playerRating);

        // assert
        Assert.Null(dto.RankDifference);
        Assert.Null(dto.BonusPoints);

        var countryDto = dto.Country;
        Assert.Null(countryDto.ActualRank);
        Assert.Equal(CountryTestData.Name, countryDto.Name);
    }

    [Fact]
    public void ToDto_WithResults_ResturnsDto()
    {
        // arrange
        var playerRating = 

        // act
        var dto = _mapper.ToDto(playerRating);

        // assert
        Assert.Equal(, dto.RankDifference);
        Assert.Equal(, dto.BonusPoints);

        var countryDto = dto.Country;
        Assert.Equal(, countryDto.ActualRank);
        Assert.Equal(CountryTestData.Name, countryDto.Name);
    }
}
