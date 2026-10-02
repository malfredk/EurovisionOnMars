using EurovisionOnMars.Dto.Countries;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Countries;

[Route("api/countries")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ILogger<CountriesController> _logger;
    private readonly ICountryService _service;

    public CountriesController(
        ILogger<CountriesController> logger,
        ICountryService service
        )
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
    {
        var countries = await _service.GetCountries();
        return Ok(countries);
    }

    [HttpPost]
    public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] NewCountryRequestDto requestDto)
    {
        var country = await _service.CreateCountry(requestDto);
        return Created(Request.Path.Value, country);
    }

    [HttpPatch("{id:int}/rank")]
    public async Task<ActionResult> UpdateCountryRank(int id, [FromBody] int rank)
    {
        await _service.SetCountryRank(id, rank);
        return NoContent();
    }
}