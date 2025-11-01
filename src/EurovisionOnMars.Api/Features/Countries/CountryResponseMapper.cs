using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.Countries;

public interface ICountryResponseMapper
{
    public CountryResponseDto ToDto(Country entity);
}

public class CountryResponseMapper : ICountryResponseMapper
{
    public CountryResponseDto ToDto(Country entity)
    {
        return new CountryResponseDto
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            ActualRank = entity.ActualRank
        };
    }
}