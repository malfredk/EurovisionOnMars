using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingMapperTest
{
    private readonly PlayerRatingMapper _mapper = new PlayerRatingMapper();

    private static readonly int ID = 34567;
    private static readonly int CATEGORY1_POINTS = 11;
    private static readonly int CATEGORY2_POINTS = 12;
    private static readonly int CATEGORY3_POINTS = 13;
    private static readonly int TOTAL_POINTS = 100;
    private static readonly int RANK = 200;
    private static readonly int COUNTRY_NUMBER = 99;
    private static readonly string COUNTRY_NAME = "heiio";

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CreateRatingEntity();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(ID, dto.Id);
        Assert.Equal(CATEGORY1_POINTS, dto.Category1Points);
        Assert.Equal(CATEGORY2_POINTS, dto.Category2Points);
        Assert.Equal(CATEGORY3_POINTS, dto.Category3Points);
        Assert.Equal(RANK, dto.Prediction.CalculatedRank);
        Assert.Equal(TOTAL_POINTS, dto.Prediction.TotalGivenPoints);
        Assert.Equal(COUNTRY_NAME, dto.Country.Name);
        Assert.Equal(COUNTRY_NUMBER, dto.Country.Number);
    }

    private PlayerRating CreateRatingEntity()
    {
        return new PlayerRating
        {
            Id = ID,
            Category1Points = CATEGORY1_POINTS,
            Category2Points = CATEGORY2_POINTS,
            Category3Points = CATEGORY3_POINTS,
            Country = CreateCountryEntity(),
            Prediction = CreatePredictionEntity(),
        };
    }

    private Country CreateCountryEntity()
    {
        return new Country
        {
            Number = COUNTRY_NUMBER,
            Name = COUNTRY_NAME,
        };
    }

    private Prediction CreatePredictionEntity()
    {
        return new Prediction
        {
            TotalGivenPoints = TOTAL_POINTS,
            CalculatedRank = RANK,
        };
    }
}
