using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryServiceTest
{
    private readonly Mock<ICountryRepository> _countryRepositoryMock;
    private readonly Mock<ILogger<CountryService>> _loggerMock;
    private readonly CountryService _service;

    public CountryServiceTest()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _loggerMock = new Mock<ILogger<CountryService>>();

        _service = new CountryService(           
            _countryRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetCountries()
    {
        // arrange
        var country1 = new Country(21, "dska");
        var country2 = new Country(4, "wf");
        var country3 = new Country(7, "jojo");
        var countries = new List<Country> 
        { 
            country1,
            country2,
            country3
        }.ToImmutableList();
        var expectedCountries = new List<Country>
        {
            country2,
            country3,
            country1
        }.ToImmutableList();

        _countryRepositoryMock.Setup(m => m.GetCountries())
            .ReturnsAsync(countries);

        // act
        var actualCountries = await _service.GetCountries();

        // assert
        Assert.Equal(expectedCountries, actualCountries);

        _countryRepositoryMock.Verify(m => m.GetCountries(), Times.Once);
    }

    [Fact]
    public async Task CreateCountry()
    {
        // arrange
        var countryRequest = CreateCountryRequest(Utils.COUNTRY_NAME, Utils.COUNTRY_NUMBER);
        var country = Utils.CreateInitialCountry();
        var expectedCountry = new Country(20, "norge");

        _countryRepositoryMock.Setup(m => m.CreateCountry(country))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.CreateCountry(countryRequest);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m => m.CreateCountry(country), Times.Once);
    }

    [Fact]
    public async Task UpdateCountry()
    {
        // arrange
        var id = 974678;
        var rank = 5;
        var existingCountry = new Country(1, "norge");
        var expectedCountry = new Country(2, "sverige");

        _countryRepositoryMock.Setup(m => m.GetCountry(id))
            .ReturnsAsync(existingCountry);
        _countryRepositoryMock.Setup(m => m.UpdateCountry(existingCountry))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.UpdateCountry(id, rank);

        // assert
        Assert.Equal(rank, existingCountry.ActualRank);

        _countryRepositoryMock.Verify(m => m.GetCountry(id), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(existingCountry), Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_InvalidId()
    {
        // arrange
        var id = 34;

        _countryRepositoryMock.Setup(m => m.GetCountry(id))
            .ReturnsAsync((Country)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.UpdateCountry(id, 3));

        _countryRepositoryMock.Verify(m => m.GetCountry(id), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
    }

    private NewCountryRequestDto CreateCountryRequest(string name, int number)
    {
        return new NewCountryRequestDto
        {
            Name = name,
            Number = number
        };
    }
}