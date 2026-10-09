using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Domain.Countries;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.Countries;

public class CountryServiceTest
{
    const int Id = 1234;

    private readonly Mock<ILogger<CountryService>> _loggerMock;
    private readonly Mock<ICountryRepository> _repositoryMock;
    private readonly Mock<ICountryMapper> _mapperMock;

    private readonly CountryService _service;

    public CountryServiceTest()
    {
        _loggerMock = new Mock<ILogger<CountryService>>();
        _repositoryMock = new Mock<ICountryRepository>();
        _mapperMock = new Mock<ICountryMapper>();

        _service = new CountryService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object
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

        var countryDtoNumber21 = CreateCountryDto(21);
        var countryDtoNumber4 = CreateCountryDto(4);
        var countryDtoNumber7 = CreateCountryDto(7);

        _mapperMock.Setup(m => m.ToDto(countryNumber21))
            .Returns(countryDtoNumber21);
        _mapperMock.Setup(m => m.ToDto(countryNumber4))
            .Returns(countryDtoNumber4);
        _mapperMock.Setup(m => m.ToDto(countryNumber7))
            .Returns(countryDtoNumber7);

        var expectedCountries = new List<CountryDto>
        {
            countryDtoNumber4,
            countryDtoNumber7,
            countryDtoNumber21
        }.ToImmutableList();

        // act
        var actualCountries = await _service.GetCountries();

        // assert
        Assert.Equal(expectedCountries, actualCountries);

        _repositoryMock.Verify(m => m.GetCountries(), Times.Once);

        _mapperMock.Verify(m => m.ToDto(countryNumber21), Times.Once);
        _mapperMock.Verify(m => m.ToDto(countryNumber4), Times.Once);
        _mapperMock.Verify(m => m.ToDto(countryNumber7), Times.Once);
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

        var expectedCountryDto = CreateCountryDto();
        _mapperMock.Setup(m => m.ToDto(expectedCountry))
            .Returns(expectedCountryDto);

        // act
        var actualCountry = await _service.CreateCountry(countryRequest);

        // assert
        Assert.Equal(expectedCountryDto, actualCountry);

        _repositoryMock.Verify(m =>
            m.CreateCountry(It.Is<Country>(c =>
                c.Number.Value == CountryTestData.Number &&
                c.Name.Value == CountryTestData.Name &&
                c.ActualRank == null)),
            Times.Once);

        _mapperMock.Verify(m => m.ToDto(expectedCountry), Times.Once);
    }

    // tests for UpdateCountry

    [Fact]
    public async Task UpdateCountry_ValidInput_UpdatesRank()
    {
        // arrange
        var rank = CountryTestData.Rank;

        var country = CountryFactory.CreateInitialCountry(1);
        _repositoryMock.Setup(m => m.GetCountry(Id))
            .ReturnsAsync(country);

        // act
        await _service.SetCountryRank(Id, rank);

        // assert
        Assert.Equal(rank, country.ActualRank!.Value);

        _repositoryMock.Verify(m => m.GetCountry(Id), Times.Once);
        _repositoryMock.Verify(m => m.UpdateCountry(country), Times.Once);
        _repositoryMock.Verify(m => m.UpdateCountry(It.Is<Country>(c => c.ActualRank!.Value == rank)), Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_InvalidId_ThrowsException()
    {
        // arrange
        _repositoryMock.Setup(m => m.GetCountry(Id))
            .ReturnsAsync((Country)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.SetCountryRank(Id, CountryTestData.Rank)
        );

        _repositoryMock.Verify(m => m.GetCountry(Id), Times.Once);
        _repositoryMock.Verify(m => m.UpdateCountry(It.IsAny<Country>()), Times.Never);
    }

    // helpers

    private NewCountryRequestDto CreateCountryRequest()
    {
        return new NewCountryRequestDto
        {
            Name = CountryTestData.Name,
            Number = CountryTestData.Number
        };
    }

    private CountryDto CreateCountryDto(int number = CountryTestData.Number)
    {
        return new CountryDto
        {
            Id = Id,
            Name = CountryTestData.Name,
            Number = number,
            ActualRank = CountryTestData.Rank
        };
    }
}