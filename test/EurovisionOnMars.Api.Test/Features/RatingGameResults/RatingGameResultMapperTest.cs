using EurovisionOnMars.Api.Features.RatingGameResults;

namespace EurovisionOnMars.Api.Test.Features.RatingGameResults;

public class RatingGameResultMapperTest
{
    private const int RANK_DIFFERENCE = 30;
    private const int BONUS_POINTS = 500;

    private readonly RatingGameResultMapper _mapper = new RatingGameResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var ratingGameResult = Utils.CreateRatingGameResult(RANK_DIFFERENCE, BONUS_POINTS);
        ratingGameResult.PlayerRating?.Country?.SetActualRank(Utils.COUNTRY_RANK);

        // act
        var dto = _mapper.ToDto(ratingGameResult);

        // assert
        Assert.Equal(RANK_DIFFERENCE, dto.RankDifference);
        Assert.Equal(BONUS_POINTS, dto.BonusPoints);
        Assert.NotNull(dto.Country);
        Assert.Equal(Utils.COUNTRY_NAME, dto.Country.Name);
        Assert.Equal(Utils.COUNTRY_RANK, dto.Country.ActualRank);
    }
}
