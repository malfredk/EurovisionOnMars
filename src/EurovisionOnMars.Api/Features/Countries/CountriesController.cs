using EurovisionOnMars.Api.Features.Common;
using EurovisionOnMars.Dto.Countries;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Countries;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _service;
    private readonly ILogger<CountriesController> _logger;
    private readonly ICountryMapper _mapper;

    public CountriesController(
        ICountryService service,
        ILogger<CountriesController> logger,
        ICountryMapper mapper
        )
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountryResponseDto>>> GetCountries()
    {
        var countries = await _service.GetCountries();
        var countryDtos = Utils.MapList(countries.ToList(), _mapper.ToDto);
        return Ok(countryDtos);
    }

    [HttpPost]
    public async Task<ActionResult<CountryResponseDto>> CreateCountry([FromBody] NewCountryRequestDto requestDto)
    {
        var country = await _service.CreateCountry(requestDto);
        var countryDto = _mapper.ToDto(country);
        return Created(Request.Path.Value, countryDto);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> UpdateCountryRank(int id, [FromBody] int rank)
    {
        await _service.UpdateCountry(id, rank);
        return Ok();
    }
}