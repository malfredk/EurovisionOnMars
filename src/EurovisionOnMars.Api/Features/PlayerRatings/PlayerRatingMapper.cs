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
            Prediction = ToPredictionDto(entity.Prediction),
            Country = ToCountryDto(entity.Country)
        };
    }

    private PredictionDto ToPredictionDto(Prediction? prediction)
    {
        if (prediction == null) {
            throw new Exception("PlayerRating is missing Prediction.");
        }

        return new PredictionDto
        {
            Id = prediction.Id,
            TotalGivenPoints = prediction.TotalGivenPoints,
            CalculatedRank = prediction.CalculatedRank,
            TieBreakDemotion = prediction.TieBreakDemotion,
            PredictedRank = prediction.GetPredictedRank()
        };
    }

    private PlayerRatingCountryDto ToCountryDto(Country? country)
    {
        if (country == null)
        {
            throw new Exception("PlayerRating is missing Country.");

        }

        return new PlayerRatingCountryDto
        {
            Number = country.Number,
            Name = country.Name
        };
    }
}