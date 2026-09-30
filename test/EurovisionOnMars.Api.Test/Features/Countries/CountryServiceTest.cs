using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Entity.Countries;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryServiceTest
{
    private readonly Mock<ILogger<CountryService>> _loggerMock;
    private readonly Mock<ICountryRepository> _repositoryMock;

    private readonly CountryService _service;

    public CountryServiceTest()
    {
        _loggerMock = new Mock<ILogger<CountryService>>();
        _repositoryMock = new Mock<ICountryRepository>();

        _service = new CountryService(
            _loggerMock.Object,
            _repositoryMock.Object
            );
    }

    // tests for GetCountries

    [Fact]
    public async Task GetCountries_ReturnsCountriesOrderedByNumber()
    {
        // arrange
        var countryNumber21 = CountryFactory.CreateInitialCountry(21);
        var countryNumber4 = CountryFactory.CreateInitialCountry(4);
        var countryNumber7 = CountryFactory.CreateInitialCountry(7);

        var countries = new List<Country> 
        { 
            countryNumber21,
            countryNumber4,
            countryNumber7
        }.ToImmutableList();
        _repositoryMock.Setup(m => m.GetCountries())
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

        _repositoryMock.Verify(m => m.GetCountries(), Times.Once);
    }

    // tests for CreateCountry

    [Fact]
    public async Task CreateCountry_ValidRequest_ReturnsNewCountry()
    {
        // arrange
        var countryRequest = CreateCountryRequest();

        var expectedCountry = CountryFactory.CreateInitialCountry();
        _repositoryMock.Setup(m => m.CreateCountry(It.IsAny<Country>()))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.CreateCountry(countryRequest);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _repositoryMock.Verify(m =>
            m.CreateCountry(It.Is<Country>(c =>
                c.Number.Value == CountryTestData.Number &&
                c.Name.Value == CountryTestData.Name)),
            Times.Once);
    }

    private NewCountryRequestDto CreateCountryRequest()
    {
        return new NewCountryRequestDto
        {
            Name = CountryTestData.Name,
            Number = CountryTestData.Number
        };
    }

    // tests for UpdateCountry

    [Fact]
    public async Task UpdateCountry_ValidInputs_UpdatesRank()
    {
        // arrange
        var fetchedCountry = CountryFactory.CreateInitialCountry(1);
        _repositoryMock.Setup(m => m.GetCountry(CountryTestData.Id))
            .ReturnsAsync(fetchedCountry);

        var expectedCountry = CountryFactory.CreateInitialCountry(2);
        _repositoryMock.Setup(m => m.UpdateCountry(fetchedCountry))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.SetCountryRank(CountryTestData.Id, CountryTestData.Rank);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        Assert.Equal(CountryTestData.Rank, fetchedCountry.ActualRank!.Value);

        _repositoryMock.Verify(m => m.GetCountry(CountryTestData.Id), Times.Once);
        _repositoryMock.Verify(m => m.UpdateCountry(fetchedCountry), Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_InvalidId_ThrowsException()
    {
        // arrange
        _repositoryMock.Setup(m => m.GetCountry(CountryTestData.Id))
            .ReturnsAsync((Country)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.SetCountryRank(CountryTestData.Id, CountryTestData.Rank)
        );

        _repositoryMock.Verify(m => m.GetCountry(CountryTestData.Id), Times.Once);
        _repositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
    }
}