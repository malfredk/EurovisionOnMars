using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.Countries.GetCountries;
using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.TestSupport.Countries;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Countries.GetCountries;

public class GetCountriesServiceTest
{
    const int Id = 1234;

    private readonly Mock<ILogger<GetCountriesService>> _loggerMock;
    private readonly Mock<IGetCountriesRepository> _repositoryMock;
    private readonly Mock<ICountryMapper> _mapperMock;

    private readonly GetCountriesService _service;

    public GetCountriesServiceTest()
    {
        _loggerMock = new Mock<ILogger<GetCountriesService>>();
        _repositoryMock = new Mock<IGetCountriesRepository>();
        _mapperMock = new Mock<ICountryMapper>();

        _service = new GetCountriesService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object
            );
    }

    [Fact]
    public async Task GetCountries_ReturnsCountriesOrderedByNumber()
    {
        // arrange
        var countryNumber21 = CountryFactory.CreateInitial(21);
        var countryNumber4 = CountryFactory.CreateInitial(4);
        var countryNumber7 = CountryFactory.CreateInitial(7);

        var countries = new List<Country> 
        { 
            countryNumber21,
            countryNumber4,
            countryNumber7
        };
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
        };

        // act
        var actualCountries = await _service.GetCountries();

        // assert
        Assert.Equal(expectedCountries, actualCountries);

        _repositoryMock.Verify(m => m.GetCountries(), Times.Once);

        _mapperMock.Verify(m => m.ToDto(countryNumber21), Times.Once);
        _mapperMock.Verify(m => m.ToDto(countryNumber4), Times.Once);
        _mapperMock.Verify(m => m.ToDto(countryNumber7), Times.Once);
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