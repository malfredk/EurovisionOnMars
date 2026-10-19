using EurovisionOnMars.Api.Features.Countries.CreateCountry;
using EurovisionOnMars.Api.Features.Countries.GetCountries;
using EurovisionOnMars.Api.Features.Countries.SetRank;
using EurovisionOnMars.Dto.Countries;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Countries;

[Route("api/countries")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ILogger<CountriesController> _logger;
    private readonly ICreateCountryService _createCountryService;
    private readonly IGetCountriesService _getCountriesService;
    private readonly ISetRankService _setRankService;

    public CountriesController(
        ILogger<CountriesController> logger,
        ICreateCountryService createCountryService,
        IGetCountriesService getCountriesService,
        ISetRankService setRankService
        )
    {
        _logger = logger;
        _createCountryService = createCountryService;
        _getCountriesService = getCountriesService;
        _setRankService = setRankService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
    {
        var countries = await _getCountriesService.GetCountries();
        return Ok(countries);
    }

    [HttpPost]
    public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] NewCountryRequestDto requestDto)
    {
        var country = await _createCountryService.CreateCountry(requestDto);
        return Created(Request.Path.Value, country);
    }

    [HttpPatch("{id:int}/rank")]
    public async Task<ActionResult> UpdateCountryRank(int id, [FromBody] int rank)
    {
        await _setRankService.SetCountryRank(id, rank);
        return NoContent();
    }
}