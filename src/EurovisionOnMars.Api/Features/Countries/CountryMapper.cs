using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Api.Features.Countries;

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
            Number = entity.Number.Value,
            Name = entity.Name.Value,
            ActualRank = entity.ActualRank?.Value
        };
    }
}