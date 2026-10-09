using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Api.Test.TestData.Players;

namespace EurovisionOnMars.Api.Test.Features.Players.GetRatings;

public class PlayerRatingMapperTest
{
    private readonly PlayerRatingMapper _mapper = new PlayerRatingMapper();

    [Fact]
    public void ToDto_InitialRating_ReturnsMostlyEmptyDto()
    {
        // arrange
        var entity = PlayerFactory.CreateInitialPlayer().PlayerRatings[0];

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
        var entity = PlayerFactory.CreatePlayerWith2RatedAndDemotedAnd2InitialRatings()
            .PlayerRatings[0];

        int expectedCategory1Points = entity.Category1Points!.Value;
        int expectedCategory2Points = entity.Category2Points!.Value;
        int expectedCategory3Points = entity.Category3Points!.Value;

        int expectedTotalGivenPoints = entity.Prediction!.TotalGivenPoints!.Value;
        int expectedCalculatedRank = entity.Prediction!.CalculatedRank!.Value;
        int expectedTieBreakDemotion = entity.Prediction!.TieBreakDemotion!.Value;
        int expectedPredictedRank = entity.Prediction!.GetPredictedRank()!.Value;

        // act
        var actual = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, actual.Id);
        Assert.Equal(expectedCategory1Points, actual.Category1Points);
        Assert.Equal(expectedCategory2Points, actual.Category2Points);
        Assert.Equal(expectedCategory3Points, actual.Category3Points);

        var actualPrediction = actual.Prediction;
        Assert.Equal(entity.Prediction.Id, actualPrediction.Id);
        Assert.Equal(expectedTotalGivenPoints, actualPrediction.TotalGivenPoints);
        Assert.Equal(expectedCalculatedRank, actualPrediction.CalculatedRank);
        Assert.Equal(expectedTieBreakDemotion, actualPrediction.TieBreakDemotion);
        Assert.Equal(expectedPredictedRank, actualPrediction.PredictedRank);

        var actualCountry = actual.Country;
        Assert.Equal(CountryTestData.Name, actualCountry.Name);
        Assert.Equal(CountryTestData.Number, actualCountry.Number);
    }
}
