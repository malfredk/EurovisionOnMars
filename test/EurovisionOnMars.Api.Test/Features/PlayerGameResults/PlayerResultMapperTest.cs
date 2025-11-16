using EurovisionOnMars.Api.Features.PlayerGameResults;

namespace EurovisionOnMars.Api.Test.Features.PlayerGameResults;

public class PlayerResultMapperTest
{
    private static int RANK = 89;
    private static int POINTS = 500;

    private readonly PlayerGameResultMapper _mapper = new PlayerGameResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = Utils.CreatePlayerGameResult(RANK, POINTS);

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(RANK, dto.Rank);
        Assert.Equal(POINTS, dto.TotalPoints);
        Assert.Equal(Utils.PLAYER_USERNAME, dto.PlayerUsername);
    }
}
