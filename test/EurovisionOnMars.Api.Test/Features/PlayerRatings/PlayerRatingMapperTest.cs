using EurovisionOnMars.Api.Features.PlayerRatings;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingMapperTest
{
    private readonly PlayerRatingMapper _mapper = new PlayerRatingMapper();

    private static readonly int CATEGORY1_POINTS = 1;
    private static readonly int CATEGORY2_POINTS = 12;
    private static readonly int CATEGORY3_POINTS = 8;
    private static readonly int RANK = 20;

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = Utils.CreatePlayerRating(CATEGORY1_POINTS, CATEGORY2_POINTS, CATEGORY3_POINTS, RANK);

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(Utils.RATING_ID, dto.Id);
        Assert.Equal(CATEGORY1_POINTS, dto.Category1Points);
        Assert.Equal(CATEGORY2_POINTS, dto.Category2Points);
        Assert.Equal(CATEGORY3_POINTS, dto.Category3Points);
        Assert.Equal(RANK, dto.Prediction.CalculatedRank);
        Assert.Equal(21, dto.Prediction.TotalGivenPoints);
        Assert.Equal(Utils.COUNTRY_NAME, dto.Country.Name);
        Assert.Equal(Utils.COUNTRY_NUMBER, dto.Country.Number);
    }
}
