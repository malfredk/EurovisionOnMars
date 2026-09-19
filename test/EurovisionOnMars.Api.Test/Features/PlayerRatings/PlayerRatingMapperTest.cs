using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingMapperTest
{
    private const int PREDICTION_ID = 55;

    private readonly PlayerRatingMapper _mapper = new PlayerRatingMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CreatePlayerRating();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(Utils.RATING_ID, dto.Id);
        Assert.Equal(Utils.CATEGORY1_POINTS.Value, dto.Category1Points);
        Assert.Equal(Utils.CATEGORY2_POINTS.Value, dto.Category2Points);
        Assert.Equal(Utils.CATEGORY3_POINTS.Value, dto.Category3Points);

        var predictionDto = dto.Prediction;
        Assert.Equal(PREDICTION_ID, predictionDto.Id);
        Assert.Equal(24, predictionDto.TotalGivenPoints);
        Assert.Equal(Utils.PREDICTION_CALCULATED_RANK.Value, predictionDto.CalculatedRank);
        Assert.Equal(Utils.TIE_BREAK_DEMOTION, predictionDto.TieBreakDemotion);
        Assert.Equal(Utils.PREDICTION_RANK.Value, predictionDto.PredictedRank);

        var countryDto = dto.Country;
        Assert.Equal(Utils.COUNTRY_NAME.Value, countryDto.Name);
        Assert.Equal(Utils.COUNTRY_NUMBER.Value, countryDto.Number);
    }

    private static PlayerRating CreatePlayerRating()
    {
        var entity = Utils.CreatePlayerRating();
        entity.Prediction.Id = PREDICTION_ID;
        return entity;
    }
}
