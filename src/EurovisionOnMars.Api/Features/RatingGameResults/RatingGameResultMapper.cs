using EurovisionOnMars.Dto.RatingGameResults;
using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultMapper
{
    public RatingGameResultDto ToDto(RatingGameResult entity);
}

public class RatingGameResultMapper : IRatingGameResultMapper
{
    public RatingGameResultDto ToDto(RatingGameResult entity)
    {
        return new RatingGameResultDto
        {
            RankDifference = entity.RankDifference,
            BonusPoints = entity.BonusPoints,
            Country = ToCountryDto(entity)
        };
    }

    private RatingGameResultCountryDto ToCountryDto(RatingGameResult ratingGameResult)
    {
        var country = ratingGameResult.PlayerRating?.Country 
            ?? throw new Exception("RatingGameResult is missing PlayerRating or related PlayerRating is missing Country.");
        return new RatingGameResultCountryDto
        {
            Name = country.Name.Value,
            ActualRank = country.ActualRank?.Value
        };
    }
}