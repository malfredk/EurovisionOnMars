using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface ICountryMapper
{
    public CountryDto ToDto(Country entity);
    public Country ToEntity(CountryDto dto);
    public Country UpdateEntity(Country entity, CountryDto dto);
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

    public Country ToEntity(CountryDto dto)
    {
        return new Country
        {
            Name = dto.Name,
            Number = dto.Number
        };
    }

    public Country UpdateEntity(Country entity, CountryDto dto)
    {
        entity.Ranking = dto.Ranking;
        return entity;
    }
}
