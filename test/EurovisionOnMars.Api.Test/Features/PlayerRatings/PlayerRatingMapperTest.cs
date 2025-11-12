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
    private static readonly int RANK = 200;

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = CreateEntity();

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(ID, dto.Id);
        Assert.Equal(CATEGORY1_POINTS, dto.Category1Points);
        Assert.Equal(CATEGORY2_POINTS, dto.Category2Points);
        Assert.Equal(CATEGORY3_POINTS, dto.Category3Points);
        Assert.Equal(RANK, dto.Prediction.CalculatedRank);
        Assert.Equal(35, dto.Prediction.TotalGivenPoints);
        Assert.Equal(Utils.COUNTRY_NAME, dto.Country.Name);
        Assert.Equal(Utils.COUNTRY_NUMBER, dto.Country.Number);
    }

    private PlayerRating CreateEntity()
    {
        var player = Utils.CreateInitialPlayerWithOneCountry();

        var rating = player.PlayerRatings.First();
        rating.SetPoints(CATEGORY1_POINTS, CATEGORY2_POINTS, CATEGORY3_POINTS);

        var prediction = rating.Prediction;
        prediction.CalculatedRank = RANK;
        
        return rating;
    }
}
