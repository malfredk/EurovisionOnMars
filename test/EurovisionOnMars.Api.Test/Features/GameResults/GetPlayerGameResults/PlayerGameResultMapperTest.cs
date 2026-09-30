using EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;
using EurovisionOnMars.Api.Test.TestData.Players;

namespace EurovisionOnMars.Api.Test.Features.GameResults.GetPlayerGameResults;

public class PlayerGameResultMapperTest
{
    private readonly PlayerGameResultMapper _mapper = new PlayerGameResultMapper();

    [Fact]
    public void ToDto_WithoutResults()
    {
        // arrange
        var entity = PlayerFactory.CreateInitialPlayer().PlayerGameResult;

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Null(dto.Rank);
        Assert.Null(dto.TotalPoints);
        Assert.Equal(PlayerTestData.Username, dto.PlayerUsername);
    }

    [Fact]
    public void ToDto_WithResults()
    {
        // arrange
        var entity = PlayerFactory.CreatePlayerAtEndOfGame().PlayerGameResult;

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.NotNull(dto.Rank);
        Assert.Equal(entity.Rank!.Value, dto.Rank);

        Assert.NotNull(dto.TotalPoints);
        Assert.Equal(entity.TotalPoints, dto.TotalPoints);

        Assert.Equal(PlayerTestData.Username, dto.PlayerUsername);
    }
}
