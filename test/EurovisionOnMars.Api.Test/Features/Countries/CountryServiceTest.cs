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
        var country1 = Utils.CreateInitialCountry(21);
        var country2 = Utils.CreateInitialCountry(4);
        var country3 = Utils.CreateInitialCountry(7);
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
        var countryRequest = CreateCountryRequest();
        var expectedCountry = Utils.CreateInitialCountry(6);

        _countryRepositoryMock.Setup(m => m.CreateCountry(It.IsAny<Country>()))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.CreateCountry(countryRequest);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m =>
            m.CreateCountry(It.Is<Country>(c =>
                c.Number == Utils.COUNTRY_NUMBER &&
                c.Name == Utils.COUNTRY_NAME)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCountry()
    {
        // arrange
        var existingCountry = Utils.CreateInitialCountry(1);
        var expectedCountry = Utils.CreateInitialCountry(2);

        _countryRepositoryMock.Setup(m => m.GetCountry(Utils.COUNTRY_ID))
            .ReturnsAsync(existingCountry);
        _countryRepositoryMock.Setup(m => m.UpdateCountry(existingCountry))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.UpdateCountry(Utils.COUNTRY_ID, Utils.COUNTRY_RANK);

        // assert
        Assert.Equal(Utils.COUNTRY_RANK, existingCountry.ActualRank);

        _countryRepositoryMock.Verify(m => m.GetCountry(Utils.COUNTRY_ID), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(existingCountry), Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_InvalidId()
    {
        // arrange
        _countryRepositoryMock.Setup(m => m.GetCountry(Utils.COUNTRY_ID))
            .ReturnsAsync((Country)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.UpdateCountry(Utils.COUNTRY_ID, Utils.COUNTRY_RANK));

        _countryRepositoryMock.Verify(m => m.GetCountry(Utils.COUNTRY_ID), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
    }

    private NewCountryRequestDto CreateCountryRequest()
    {
        return new NewCountryRequestDto
        {
            Name = Utils.COUNTRY_NAME,
            Number = Utils.COUNTRY_NUMBER
        };
    }
}