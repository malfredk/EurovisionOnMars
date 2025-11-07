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
        var country1 = CreateCountry("dska", 21);
        var country2 = CreateCountry("wf", 4);
        var country3 = CreateCountry("jojo", 7);
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

    [Theory]
    [InlineData("østerrike", 1)]
    [InlineData("san marino", 26)]
    [InlineData("bosnia-hercegovina", 13)]
    public async Task CreateCountry_Valid(string name, int number)
    {
        // arrange
        var countryRequest = CreateCountryRequest(name, number);
        var country = CreateCountry(name, number);
        var expectedCountry = CreateCountry("dska", 90);

        _countryRepositoryMock.Setup(m => m.CreateCountry(country))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.CreateCountry(countryRequest);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m => m.CreateCountry(country), Times.Once);
    }

    [Theory]
    [InlineData("Danmark", 1)]
    [InlineData("england", 1)]
    [InlineData("danmark2", 26)]
    [InlineData("danmark",0)]
    [InlineData("danmark",-10)]
    [InlineData("danmark",27)]
    [InlineData("danmark_",1)]
    [InlineData("",1)]
    public async Task CreateCountry_Invalid(string name, int number)
    {
        // arrange
        var countryRequest = CreateCountryRequest(name, number);

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateCountry(countryRequest));

        _countryRepositoryMock.Verify(m => m.CreateCountry(It.IsAny<Country>()), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(26)]
    [InlineData(13)]
    public async Task UpdateCountry_Valid(int rank)
    {
        // arrange
        var id = 974678;
        var existingCountry = CreateCountry(null);
        var updatedCountry = CreateCountry(rank);
        var expectedCountry = CreateCountry("dska", 90);

        _countryRepositoryMock.Setup(m => m.GetCountry(id))
            .ReturnsAsync(existingCountry);
        _countryRepositoryMock.Setup(m => m.UpdateCountry(existingCountry))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.UpdateCountry(id, rank);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m => m.GetCountry(id), Times.Once);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(existingCountry), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(27)]
    public async Task UpdateCountry_Invalid(int rank)
    {
        // arrange
        var id = 974678;

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateCountry(id, rank));

        _countryRepositoryMock.Verify(m => m.GetCountry(It.IsAny<int>()), Times.Never);
        _countryRepositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
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

    private Country CreateCountry(string name, int number)
    {
        return new Country
        {
            Name = name,
            Number = number
        };
    }

    private Country CreateCountry(int? rank)
    {
        return new Country
        {
            Name = "danmark",
            Number = 1,
            ActualRank = rank
        };
    }
}