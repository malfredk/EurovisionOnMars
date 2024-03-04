using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Mappers;

public class PlayerResultMapperTest
{
    private readonly PlayerResultMapper _mapper = new PlayerResultMapper();

    [Fact]
    public void ToDto()
    {
        // arrange
        var entity = new PlayerResult
        {
            Id = 82,
            Ranking = 28,
            Score = -23,
            PlayerId = 1,
            Player = new Player { Id = 2, Username = "te" }
        };

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Ranking, dto.Ranking);
        Assert.Equal(entity.Score, dto.Score);
    }
}
