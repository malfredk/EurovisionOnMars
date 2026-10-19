using EurovisionOnMars.Api.Features.Countries.SetRank;
using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.TestSupport.Countries;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Countries.SetRank;

public class SetRankServiceTest
{
    const int Id = 1234;

    private readonly Mock<ILogger<SetRankService>> _loggerMock;
    private readonly Mock<ISetRankRepository> _repositoryMock;

    private readonly SetRankService _service;

    public SetRankServiceTest()
    {
        _loggerMock = new Mock<ILogger<SetRankService>>();
        _repositoryMock = new Mock<ISetRankRepository>();

        _service = new SetRankService(
            _loggerMock.Object,
            _repositoryMock.Object
            );
    }

    [Fact]
    public async Task UpdateCountry_ValidInput_UpdatesRank()
    {
        // arrange
        var rank = CountryTestData.Rank;

        var country = CountryFactory.CreateInitial();
        _repositoryMock.Setup(m => m.GetCountry(Id))
            .ReturnsAsync(country);

        // act
        await _service.SetCountryRank(Id, rank);

        // assert
        Assert.Equal(rank, country.ActualRank!.Value);

        _repositoryMock.Verify(m => m.GetCountry(Id), Times.Once);
        _repositoryMock.Verify(m => m.UpdateCountry(country), Times.Once);
        _repositoryMock.Verify(m => 
            m.UpdateCountry(It.Is<Country>(c => c.ActualRank!.Value == rank)), 
            Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_InvalidId_ThrowsAndDoesNotUpdate()
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
}