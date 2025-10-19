using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IPredictionMapper
{
    public PredictionDto ToDto(Prediction entity);
}

public class PredictionMapper : IPredictionMapper
{
    public PredictionDto ToDto(Prediction entity)
    {
        return new PredictionDto
        {
            Id = entity.Id,
            TotalGivenPoints = entity.TotalGivenPoints,
            CalculatedRank = entity.CalculatedRank
        };
    }
}
