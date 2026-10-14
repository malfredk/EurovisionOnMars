using EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;
using EurovisionOnMars.TestSupport.Players;

namespace EurovisionOnMars.Api.Test.Features.GameResults.GetPlayerGameResults;

public class PlayerGameResultMapperTest
{
    private readonly PlayerGameResultMapper _mapper = new PlayerGameResultMapper();

    [Fact]
    public void ToDto_WithoutResults_ReturnsDtoWithOnlyUsername()
    {
        // arrange
        var entity = PlayerFactory.CreateInitial(ratingCount: 1)
            .PlayerGameResult;

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Null(dto.Rank);
        Assert.Null(dto.TotalPoints);
        Assert.Equal(PlayerTestData.Username, dto.PlayerUsername);
    }

    [Fact]
    public void ToDto_WithResults_ReturnsCompleteDto()
    {
        // arrange
        var rank = 1;
        var totalPoints = 9;
        var entity = PlayerFactory.CreateWithCompletePlayerGameResult(totalPoints, rank)
            .PlayerGameResult;

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(rank, dto.Rank);
        Assert.Equal(totalPoints, dto.TotalPoints);
        Assert.Equal(PlayerTestData.Username, dto.PlayerUsername);
    }
}
