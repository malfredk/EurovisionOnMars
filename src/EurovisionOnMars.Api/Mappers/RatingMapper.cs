using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IRatingMapper
{
    public RatingDto ToDto(Rating entity);
}

public class RatingMapper : IRatingMapper
{
    private readonly ICountryMapper _countryMapper;
    private readonly IRatingResultMapper _ratingResultMapper;

    public RatingMapper(ICountryMapper countryMapper, IRatingResultMapper ratingResultMapper)
    {
        _countryMapper = countryMapper;
        _ratingResultMapper = ratingResultMapper;
    }

    public RatingDto ToDto(Rating entity)
    {
        return new RatingDto
        {
            Id = entity.Id,
            Category1Points = entity.Category1Points,
            Category2Points = entity.Category2Points,
            Category3Points = entity.Category3Points,
            PlayerId = entity.PlayerId,
            PointsSum = entity.PointsSum,
            Ranking = entity.Ranking,
            CountryId = entity.CountryId,
            Country = entity.Country is null ? null : _countryMapper.ToDto(entity.Country),
            RatingResult = entity.RatingResult is null ? null : _ratingResultMapper.ToDto(entity.RatingResult)
        };
    }
}