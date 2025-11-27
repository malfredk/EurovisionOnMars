using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingMapper
{
    public PlayerRatingResponseDto ToDto(PlayerRating entity);
}

public class PlayerRatingMapper : IPlayerRatingMapper
{
    public PlayerRatingResponseDto ToDto(PlayerRating entity)
    {
        return new PlayerRatingResponseDto
        {
            Id = entity.Id,
            Category1Points = entity.Category1Points,
            Category2Points = entity.Category2Points,
            Category3Points = entity.Category3Points,
            Prediction = ToPredicitionDto(entity),
            Country = ToCountryDto(entity)
        };
    }

    private PredictionResponseDto ToPredicitionDto(PlayerRating rating)
    {
        Prediction prediction = rating.Prediction;
        return new PredictionResponseDto
        {
            TotalGivenPoints = prediction.TotalGivenPoints,
            CalculatedRank = prediction.CalculatedRank
        };
    }

    private PlayerRatingCountryResponseDto ToCountryDto(PlayerRating rating)
    {
        var country = rating.Country ?? 
            throw new Exception("PlayerRating is missing Country");
        return new PlayerRatingCountryResponseDto
        {
            Number = country.Number,
            Name = country.Name
        };
    }
}