using EurovisionOnMars.Api.Features;
using EurovisionOnMars.Api.Features.Predictions;
using EurovisionOnMars.Dto.Predictions;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Predictions;

public class PredictionServiceTest
{
    private readonly Mock<ILogger<PredictionService>> _loggerMock;
    private readonly Mock<IRatingTimeValidator> _ratingTimeValidatorMock;
    private readonly Mock<IPredictionRepository> _repositoryMock;

    private readonly PredictionService _service;

    public PredictionServiceTest()
    {
        _loggerMock = new Mock<ILogger<PredictionService>>();
        _ratingTimeValidatorMock = new Mock<IRatingTimeValidator>();
        _repositoryMock = new Mock<IPredictionRepository>();

        _service = new PredictionService(
            _loggerMock.Object,
            _ratingTimeValidatorMock.Object,
            _repositoryMock.Object
            );
    }


    [Fact]
    public async Task UpdateTieBreakDemotions()
    {
        // arrange
        var firstId = 8;
        var secondId = 5;
        var thirdId = 7;
        var orderedPredicionIds = new int[] { firstId, secondId, thirdId };
        var request = CreateRequest(orderedPredicionIds);

        var firstPrediction = CreatePrediction(firstId);
        _repositoryMock
            .Setup(r => r.GetPrediction(firstId)).ReturnsAsync(firstPrediction);

        var secondPrediction = CreatePrediction(secondId);
        var thirdPrediction = CreatePrediction(thirdId);
        var predictions = new List<Prediction> { thirdPrediction, firstPrediction, secondPrediction };
        _repositoryMock
            .Setup(r => r.GetTiedPredictions(Utils.PLAYER_ID, Utils.PREDICTION_CALCULATED_RANK))
            .ReturnsAsync(predictions);

        // act
        await _service.UpdateTieBreakDemotions(request);

        // assert
        Assert.Equal(0, firstPrediction.TieBreakDemotion);
        Assert.Equal(1, secondPrediction.TieBreakDemotion);
        Assert.Equal(2, thirdPrediction.TieBreakDemotion);

        _ratingTimeValidatorMock.Verify(v => v.EnsureRatingIsOpen(), Times.Once);

        _repositoryMock.Verify(r => r.GetPrediction(firstId), Times.Once);
        _repositoryMock.Verify(r => r.GetTiedPredictions(Utils.PLAYER_ID, Utils.PREDICTION_CALCULATED_RANK), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }


    [Theory]
    [InlineData(new int[] { 3, 7, 3, 5 })]
    [InlineData(new int[] { 3 })]
    public async Task UpdateTieBreakDemotions_InvalidRequest(int[] orderedPredicionIds)
    {
        // arrange
        var request = CreateRequest(orderedPredicionIds);
   
        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.UpdateTieBreakDemotions(request)
        );

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateTieBreakDemotions_InvalidFirstId()
    {
        // arrange
        var firstId = 8;
        var orderedPredicionIds = new int[] { firstId, 2 };
        var request = CreateRequest(orderedPredicionIds);

        _repositoryMock
            .Setup(r => r.GetPrediction(firstId)).ReturnsAsync((Prediction)null);


        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.UpdateTieBreakDemotions(request)
        );

        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task UpdateTieBreakDemotions_UnrankedFirstPrediction()
    {
        // arrange
        var firstId = 8;
        var orderedPredicionIds = new int[] { firstId, 2 };
        var request = CreateRequest(orderedPredicionIds);

        var firstPrediction = CreateInitialPrediciton();
        _repositoryMock
            .Setup(r => r.GetPrediction(firstId)).ReturnsAsync(firstPrediction);


        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.UpdateTieBreakDemotions(request)
        );

        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Theory]
    [InlineData(new int[] { 34, 87 })]
    [InlineData(new int[] { 34 })]
    [InlineData(new int[] { 34, 80, 990 })]
    public async Task UpdateTieBreakDemotions_NotTiedIds(int[] requestNotFirstIds)
    {
        // arrange
        var firstId = 8;
        var orderedPredicionIds = requestNotFirstIds.Prepend(firstId).ToArray();
        var request = CreateRequest(orderedPredicionIds);

        var firstPrediction = CreatePrediction(firstId);
        _repositoryMock
            .Setup(r => r.GetPrediction(firstId)).ReturnsAsync(firstPrediction);

        var secondPrediction = CreatePrediction(34);
        var thirdPrediction = CreatePrediction(80);
        var predictions = new List<Prediction> { thirdPrediction, firstPrediction, secondPrediction };
        _repositoryMock
            .Setup(r => r.GetTiedPredictions(Utils.PLAYER_ID, Utils.PREDICTION_CALCULATED_RANK))
            .ReturnsAsync(predictions);

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.UpdateTieBreakDemotions(request)
        );

        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    private static ResolveTieBreakRequestDto CreateRequest(int[] orderedPredicionIds)
    {
        return new ResolveTieBreakRequestDto
        {
            OrderedPredictionIds = orderedPredicionIds.ToList()
        };
    }

    private static Prediction CreateInitialPrediciton()
    {
        return Utils.CreateInitialPlayerRating().Prediction;
    }

    private static Prediction CreatePrediction(int id)
    {
        var prediction = CreateInitialPrediciton();
        prediction.SetCalculatedRank(Utils.PREDICTION_CALCULATED_RANK);
        prediction.Id = id;
        return prediction;
    }
}
