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
            Prediction = ToPredicitionDto(entity.Prediction),
            Country = ToCountryDto(entity.Country)
        };
    }

    private PredictionResponseDto ToPredicitionDto(Prediction entity)
    {
        return new PredictionResponseDto
        {
            TotalGivenPoints = entity.TotalGivenPoints,
            CalculatedRank = entity.CalculatedRank
        };
    }

    private CountryResponseDto ToCountryDto(Country entity)
    {
        return new CountryResponseDto
        {
            Number = entity.Number,
            Name = entity.Name
        };
    }
}