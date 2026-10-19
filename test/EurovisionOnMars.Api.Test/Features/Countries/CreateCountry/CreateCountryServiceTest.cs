using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.Countries.CreateCountry;
using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.TestSupport.Countries;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Countries.CreateCountry;

public class CreateCountryServiceTest
{
    const int Id = 1234;

    private readonly Mock<ILogger<CreateCountryService>> _loggerMock;
    private readonly Mock<ICreateCountryRepository> _repositoryMock;
    private readonly Mock<ICountryMapper> _mapperMock;

    private readonly CreateCountryService _service;

    public CreateCountryServiceTest()
    {
        _loggerMock = new Mock<ILogger<CreateCountryService>>();
        _repositoryMock = new Mock<ICreateCountryRepository>();
        _mapperMock = new Mock<ICountryMapper>();

        _service = new CreateCountryService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object
            );
    }

    [Fact]
    public async Task CreateCountry_ValidRequest_ReturnsNewCountry()
    {
        // arrange
        var countryRequest = CreateCountryRequest();

        var countryEntity = CountryFactory.CreateInitial();
        _repositoryMock.Setup(m => m.CreateCountry(It.IsAny<Country>()))
            .ReturnsAsync(countryEntity);

        var expectedCountryDto = CreateCountryDto();
        _mapperMock.Setup(m => m.ToDto(countryEntity))
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

        _mapperMock.Verify(m => m.ToDto(countryEntity), Times.Once);
    }

    [Fact]
    public async Task CreateCountry_InvalidRequest_ThrowsAndDoesNotCreate()
    {
        // arrange
        var invalidCountryRequest = CreateCountryRequest(0);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.CreateCountry(invalidCountryRequest)
        );

        _repositoryMock.Verify(m => m.CreateCountry(It.IsAny<Country>()), Times.Never);
        _mapperMock.Verify(m => m.ToDto(It.IsAny<Country>()), Times.Never);
    }

    private NewCountryRequestDto CreateCountryRequest(int number = CountryTestData.Number)
    {
        return new NewCountryRequestDto
        {
            Name = CountryTestData.Name,
            Number = number
        };
    }

    private CountryDto CreateCountryDto()
    {
        return new CountryDto
        {
            Id = Id,
            Name = CountryTestData.Name,
            Number = CountryTestData.Number,
            ActualRank = CountryTestData.Rank
        };
    }
}