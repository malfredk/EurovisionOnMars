using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

public class PredicitonMapperTest
{
    private readonly PredictionMapper _mapper = new PredictionMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = new Prediction
        {
            Id = 56,
            TotalGivenPoints = 123,
            CalculatedRank = 4,
            PlayerRatingId = 1,
            PlayerRating = new PlayerRating { Id = 877 }
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.TotalGivenPoints, dto.TotalGivenPoints);
        Assert.Equal(entity.CalculatedRank, dto.CalculatedRank);
    }
}