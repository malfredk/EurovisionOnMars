using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.TestSupport.Countries;
using EurovisionOnMars.TestSupport.Players;

namespace EurovisionOnMars.Api.Test.Features.Players.GetRatings;

public class PlayerRatingMapperTest
{
    private readonly PlayerRatingMapper _mapper = new PlayerRatingMapper();

    [Fact]
    public void ToDto_InitialRating_ReturnsMostlyEmptyDto()
    {
        // arrange
        var entity = PlayerFactory.CreateInitialWithOneRating().PlayerRatings[0];

        // act
        var actual = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, actual.Id);
        Assert.Null(actual.Category1Points);
        Assert.Null(actual.Category2Points);
        Assert.Null(actual.Category3Points);

        var actualPrediction = actual.Prediction;
        Assert.Equal(entity.Id, actualPrediction.Id);
        Assert.Null(actualPrediction.TotalGivenPoints);
        Assert.Null(actualPrediction.CalculatedRank);
        Assert.Null(actualPrediction.TieBreakDemotion);
        Assert.Null(actualPrediction.PredictedRank);

        var actualCountry = actual.Country;
        Assert.Equal(CountryTestData.Name, actualCountry.Name);
        Assert.Equal(CountryTestData.Number, actualCountry.Number);
    }

    [Fact]
    public void ToDto_RatedRating_ReturnsMappedDto()
    {
        // arrange
        var entity = PlayerFactory.CreateWithRatedRatingAndCalculatedPrediction()
            .PlayerRatings[0];

        // act
        var actual = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, actual.Id);
        Assert.Equal(PlayerRatingTestData.Category1Points, actual.Category1Points);
        Assert.Equal(PlayerRatingTestData.Category2Points, actual.Category2Points);
        Assert.Equal(PlayerRatingTestData.Category3Points, actual.Category3Points);

        var actualPrediction = actual.Prediction;
        Assert.Equal(entity.Prediction.Id, actualPrediction.Id);
        Assert.Equal(PredictionTestData.TotalGivenPoints, actualPrediction.TotalGivenPoints);
        Assert.Equal(PredictionTestData.CalculatedRank, actualPrediction.CalculatedRank);
        Assert.Equal(PredictionTestData.TieBreakDemotion, actualPrediction.TieBreakDemotion);
        Assert.Equal(15, actualPrediction.PredictedRank);

        var actualCountry = actual.Country;
        Assert.Equal(CountryTestData.Name, actualCountry.Name);
        Assert.Equal(CountryTestData.Number, actualCountry.Number);
    }
}
