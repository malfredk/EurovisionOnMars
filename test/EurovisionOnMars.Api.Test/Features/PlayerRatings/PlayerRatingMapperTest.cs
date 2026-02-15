using EurovisionOnMars.Api.Features.PlayerRatings;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingMapperTest
{
    private readonly PlayerRatingMapper _mapper = new PlayerRatingMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = Utils.CreatePlayerRating();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(Utils.RATING_ID, dto.Id);
        Assert.Equal(Utils.CATEGORY1_POINTS, dto.Category1Points);
        Assert.Equal(Utils.CATEGORY2_POINTS, dto.Category2Points);
        Assert.Equal(Utils.CATEGORY3_POINTS, dto.Category3Points);

        var predictionDto = dto.Prediction;
        Assert.Equal(Utils.PREDICTION_ID, predictionDto.Id);
        Assert.Equal(Utils.PREDICTION_TOTAL_POINTS, predictionDto.TotalGivenPoints);
        Assert.Equal(Utils.PREDICTION_CALCULATED_RANK, predictionDto.CalculatedRank);
        Assert.Equal(Utils.TIE_BREAK_DEMOTION, predictionDto.TieBreakDemotion);
        Assert.Equal(Utils.PREDICTION_RANK, predictionDto.PredictedRank);

        var countryDto = dto.Country;
        Assert.Equal(Utils.COUNTRY_NAME, countryDto.Name);
        Assert.Equal(Utils.COUNTRY_NUMBER, countryDto.Number);
    }
}
