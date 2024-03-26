using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Controllers;

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
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
    {
        var countries = await _service.GetCountries();
        var countryDtos = Utils.MapList(countries.ToList(), _mapper.ToDto);
        return Ok(countryDtos);
    }

    [HttpPost]
    public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] NewCountryRequestDto requestDto)
    {
        var country = await _service.CreateCountry(requestDto);
        var countryDto = _mapper.ToDto(country);
        return Created(Request.Path.Value, countryDto);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateCountryRanking(int id, [FromBody] int ranking)
    {
        await _service.UpdateCountry(id, ranking);
        return Ok();
    }
}