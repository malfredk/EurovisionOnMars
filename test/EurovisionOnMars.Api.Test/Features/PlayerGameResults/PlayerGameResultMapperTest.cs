using EurovisionOnMars.Api.Features.GameResults.GetResults;

namespace EurovisionOnMars.Api.Test.Features.PlayerGameResults;

public class PlayerGameResultMapperTest
{
    private static int RANK = 89;
    private static int POINTS = 500;

    private readonly GetPlayerResultsMapper _mapper = new GetPlayerResultsMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = Utils.CreatePlayerGameResult(new(RANK), POINTS);

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(RANK, dto.Rank);
        Assert.Equal(POINTS, dto.TotalPoints);
        Assert.Equal(Utils.PLAYER_USERNAME.Value, dto.PlayerUsername);
    }
}
