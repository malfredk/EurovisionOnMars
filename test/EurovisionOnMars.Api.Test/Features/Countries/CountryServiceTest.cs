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

    // tests for GetCountries

    [Fact]
    public async Task GetCountries()
    {
        // arrange
        var countryNumber21 = Utils.CreateInitialCountry(21);
        var countryNumber4 = Utils.CreateInitialCountry(4);
        var countryNumber7 = Utils.CreateInitialCountry(7);

        var countries = new List<Country> 
        { 
            countryNumber21,
            countryNumber4,
            countryNumber7
        }.ToImmutableList();
        _countryRepositoryMock.Setup(m => m.GetCountries())
            .ReturnsAsync(countries);

        var expectedCountries = new List<Country>
        {
            countryNumber4,
            countryNumber7,
            countryNumber21
        }.ToImmutableList();

        // act
        var actualCountries = await _service.GetCountries();

        // assert
        Assert.Equal(expectedCountries, actualCountries);

        _countryRepositoryMock.Verify(m => m.GetCountries(), Times.Once);
    }

    // tests for CreateCountry

    [Fact]
    public async Task CreateCountry()
    {
        // arrange
        var countryRequest = CreateCountryRequest();

        var expectedCountry = Utils.CreateInitialCountry();
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

    // tests for UpdateCountry

    [Fact]
    public async Task UpdateCountry()
    {
        // arrange
        var fetchedCountry = Utils.CreateInitialCountry(1);
        _countryRepositoryMock.Setup(m => m.GetCountry(Utils.COUNTRY_ID))
            .ReturnsAsync(fetchedCountry);

        var expectedCountry = Utils.CreateInitialCountry(2);
        _countryRepositoryMock.Setup(m => m.UpdateCountry(fetchedCountry))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.UpdateCountry(Utils.COUNTRY_ID, Utils.COUNTRY_RANK);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        Assert.Equal(Utils.COUNTRY_RANK, fetchedCountry.ActualRank);

        _countryRepositoryMock.Verify(m => m.GetCountry(Utils.COUNTRY_ID), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(fetchedCountry), Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_InvalidId()
    {
        // arrange
        _countryRepositoryMock.Setup(m => m.GetCountry(Utils.COUNTRY_ID))
            .ReturnsAsync((Country)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.UpdateCountry(Utils.COUNTRY_ID, Utils.COUNTRY_RANK)
        );

        _countryRepositoryMock.Verify(m => m.GetCountry(Utils.COUNTRY_ID), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
    }

    // helper methods

    private NewCountryRequestDto CreateCountryRequest()
    {
        return new NewCountryRequestDto
        {
            Name = Utils.COUNTRY_NAME,
            Number = Utils.COUNTRY_NUMBER
        };
    }
}