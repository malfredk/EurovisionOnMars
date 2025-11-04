using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Features.PlayerGameResults;

public class PlayerResultMapperTest
{
    private readonly PlayerGameResultMapper _mapper = new PlayerGameResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = new PlayerGameResult
        {
            Id = 82,
            Rank = 28,
            TotalPoints = -23,
            PlayerId = 1,
            Player = new Player { Id = 2, Username = "te" }
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Rank, dto.Rank);
        Assert.Equal(entity.TotalPoints, dto.TotalPoints);
        Assert.Equal(entity.Player?.Username, dto.PlayerUsername);
    }
}
