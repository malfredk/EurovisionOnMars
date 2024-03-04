using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

public class RatingResultMapperTest
{
    private readonly RatingResultMapper _mapper = new RatingResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = new RatingResult
        {
            Id = 56,
            RankingDifference = -45,
            BonusPoints = 25,
            RatingId = 1,
            Rating = new Rating { Id = 877 }
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.RankingDifference, dto.RankingDifference);
        Assert.Equal(entity.BonusPoints, dto.BonusPoints);
    }
}