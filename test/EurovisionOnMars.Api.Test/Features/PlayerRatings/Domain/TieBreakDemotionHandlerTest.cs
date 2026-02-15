using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings.Domain;

public class TieBreakDemotionHandlerTest
{
    private readonly Mock<ILogger<TieBreakDemotionHandler>> _loggerMock;
    private readonly TieBreakDemotionHandler _handler;

    public TieBreakDemotionHandlerTest()
    {
        _loggerMock = new Mock<ILogger<TieBreakDemotionHandler>>();
        _handler = new TieBreakDemotionHandler(_loggerMock.Object);
    }

    [Fact]
    public void CalculateTieBreakDemotions()
    {
        // arrange

        // act

        // assert
        // TODO
    }
}
