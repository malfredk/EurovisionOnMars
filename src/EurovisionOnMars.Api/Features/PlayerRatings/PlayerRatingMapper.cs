using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingMapper
{
    public PlayerRatingDto ToDto(PlayerRating entity);
}

public class PlayerRatingMapper : IPlayerRatingMapper
{
    public PlayerRatingDto ToDto(PlayerRating entity)
    {
        return new PlayerRatingDto
        {
            Id = entity.Id,
            Category1Points = entity.Category1Points,
            Category2Points = entity.Category2Points,
            Category3Points = entity.Category3Points,
            Prediction = ToPredicitionDto(entity),
            Country = ToCountryDto(entity)
        };
    }

    private PredictionDto ToPredicitionDto(PlayerRating rating)
    {
        Prediction prediction = rating.Prediction;
        return new PredictionDto
        {
            TotalGivenPoints = prediction.TotalGivenPoints,
            CalculatedRank = prediction.CalculatedRank
        };
    }

    private PlayerRatingCountryDto ToCountryDto(PlayerRating rating)
    {
        var country = rating.Country ?? 
            throw new Exception("PlayerRating is missing Country");
        return new PlayerRatingCountryDto
        {
            Number = country.Number,
            Name = country.Name
        };
    }
}