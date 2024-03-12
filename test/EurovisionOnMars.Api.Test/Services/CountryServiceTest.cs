using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

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
    public async void GetCountries()
    {
        // arrange
        var expectedCountries = new List<Country> 
        { 
            CreateCountry("dska", 90), 
            CreateCountry("wf", -3) 
        }.ToImmutableList();

        _countryRepositoryMock.Setup(m => m.GetCountries())
            .ReturnsAsync(expectedCountries);

        // act
        var actualCountries = await _service.GetCountries();

        // assert
        Assert.Equal(expectedCountries, actualCountries);

        _countryRepositoryMock.Verify(m => m.GetCountries(), Times.Once);
    }

    [Fact]
    public async void GetCountry()
    {
        // arrange
        var id = 34;
        var expectedCountry = CreateCountry("dska", 90);

        _countryRepositoryMock.Setup(m => m.GetCountry(id))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.GetCountry(id);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m => m.GetCountry(id), Times.Once);
    }

    [Fact]
    public async void GetCountry_InvalidId()
    {
        // arrange
        var id = 34;

        _countryRepositoryMock.Setup(m => m.GetCountry(id))
            .ReturnsAsync((Country)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetCountry(id));

        _countryRepositoryMock.Verify(m => m.GetCountry(id), Times.Once);
    }

    [Theory]
    [InlineData("østerrike", 1)]
    [InlineData("san marino", 26)]
    [InlineData("bosnia-hercegovina", 13)]
    public async void CreateCountry_Valid(string name, int number)
    {
        // arrange
        var country = CreateCountry(name, number);
        var expectedCountry = CreateCountry("dska", 90);

        _countryRepositoryMock.Setup(m => m.CreateCountry(country))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.CreateCountry(country);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m => m.CreateCountry(country), Times.Once);
    }

    [Theory]
    [InlineData("Danmark", 1)]
    [InlineData("danmark2", 26)]
    [InlineData("danmark",0)]
    [InlineData("danmark",-10)]
    [InlineData("danmark",27)]
    [InlineData("danmark_",1)]
    [InlineData("",1)]
    public async void CreateCountry_InValid(string name, int number)
    {
        // arrange
        var country = CreateCountry(name, number);

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateCountry(country));

        _countryRepositoryMock.Verify(m => m.CreateCountry(It.IsAny<Country>()), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(26)]
    [InlineData(13)]
    public async void UpdateCountry_Valid(int? ranking)
    {
        // arrange
        var country = CreateCountry(ranking);
        var expectedCountry = CreateCountry("dska", 90);

        _countryRepositoryMock.Setup(m => m.UpdateCountry(country))
            .ReturnsAsync(expectedCountry);

        // act
        var actualCountry = await _service.UpdateCountry(country);

        // assert
        Assert.Equal(expectedCountry, actualCountry);

        _countryRepositoryMock.Verify(m => m.UpdateCountry(country), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(27)]
    [InlineData(null)]
    public async void UpdateCountry_InValid(int? ranking)
    {
        // arrange
        var country = CreateCountry(ranking);

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateCountry(country));

        _countryRepositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
    }

    private Country CreateCountry(string name, int number)
    {
        return new Country
        {
            Name = name,
            Number = number
        };
    }

    private Country CreateCountry(int? ranking)
    {
        return new Country
        {
            Name = "danmark",
            Number = 1,
            Ranking = ranking
        };
    }
}