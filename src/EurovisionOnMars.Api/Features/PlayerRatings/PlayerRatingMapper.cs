using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingMapper
{
    public PlayerRatingResponseDto ToDto(PlayerRating entity);
}

public class PlayerRatingMapper : IPlayerRatingMapper
{
    private readonly ICountryMapper _countryMapper;
    private readonly IRatingGameResultMapper _ratingResultMapper;
    private readonly IPredictionMapper _predictionMapper;

    public PlayerRatingMapper(
        ICountryMapper countryMapper, 
        IRatingGameResultMapper ratingResultMapper,
        IPredictionMapper predictionMapper
        )
    {
        _countryMapper = countryMapper;
        _ratingResultMapper = ratingResultMapper;
        _predictionMapper = predictionMapper;
    }

    public PlayerRatingResponseDto ToDto(PlayerRating entity)
    {
        return new PlayerRatingResponseDto
        {
            Id = entity.Id,
            Category1Points = entity.Category1Points,
            Category2Points = entity.Category2Points,
            Category3Points = entity.Category3Points,
            Prediction = entity.Prediction is null ? null : _predictionMapper.ToDto(entity.Prediction),
            Country = entity.Country is null ? null : _countryMapper.ToDto(entity.Country),
            RatingGameResult = entity.RatingGameResult is null ? null : _ratingResultMapper.ToDto(entity.RatingGameResult)
        };
    }
}