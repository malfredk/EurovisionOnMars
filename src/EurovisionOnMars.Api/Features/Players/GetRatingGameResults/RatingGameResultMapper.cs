using EurovisionOnMars.Dto.RatingGameResults;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Features.Players.GetRatingGameResults;

public interface IRatingGameResultMapper
{
    public RatingGameResultDto ToDto(PlayerRating rating);
}

public class RatingGameResultMapper : IRatingGameResultMapper
{
    public RatingGameResultDto ToDto(PlayerRating rating)
    {
        var result = rating.RatingGameResult;
        return new RatingGameResultDto
        {
            RankDifference = result.RankDifference,
            BonusPoints = result.BonusPoints?.Value,
            Country = ToCountryDto(rating)
        };
    }

    private RatingGameResultCountryDto ToCountryDto(PlayerRating rating)
    {
        var country = rating.Country 
            ?? throw new Exception("PlayerRating is missing Country.");
        return new RatingGameResultCountryDto
        {
            Name = country.Name.Value,
            ActualRank = country.ActualRank?.Value
        };
    }
}