using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface ICountryMapper
{
    public CountryDto ToDto(Country entity);
}

public class CountryMapper : ICountryMapper
{
    public CountryDto ToDto(Country entity)
    {
        return new CountryDto
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            Ranking = entity.Ranking
        };
    }
}
