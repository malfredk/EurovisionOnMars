using EurovisionOnMars.Dto.RatingGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultMapper
{
    public RatingGameResultResponseDto ToDto(RatingGameResult entity);
}

public class RatingGameResultMapper : IRatingGameResultMapper
{
    public RatingGameResultResponseDto ToDto(RatingGameResult entity)
    {
        return new RatingGameResultResponseDto
        {
            RankDifference = entity.RankDifference,
            BonusPoints = entity.BonusPoints,
            Country = ToCountryDto(entity)
        };
    }

    private CountryResponseDto ToCountryDto(RatingGameResult ratingGameResult)
    {
        var country = ratingGameResult.PlayerRating?.Country 
            ?? throw new Exception("RatingGameResult's related PlayerRating is missing Country.");
        return new CountryResponseDto
        {
            Name = country.Name,
            ActualRank = country.ActualRank
        };
    }
}