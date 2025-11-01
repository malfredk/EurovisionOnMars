using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

public class RatingResultMapperTest
{
    private readonly RatingGameResultMapper _mapper = new RatingGameResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = new RatingGameResult
        {
            Id = 56,
            RankDifference = -45,
            BonusPoints = 25,
            PlayerRatingId = 1,
            PlayerRating = new PlayerRating { Id = 877 }
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.RankDifference, dto.RankDifference);
        Assert.Equal(entity.BonusPoints, dto.BonusPoints);
    }
}