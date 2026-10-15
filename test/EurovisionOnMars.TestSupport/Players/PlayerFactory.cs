using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Players.Predictions;
using EurovisionOnMars.TestSupport.Countries;

namespace EurovisionOnMars.TestSupport.Players;

public static class PlayerFactory
{
    public static Player CreateInitialWithOneRating()
    {
        var countries = CountryFactory.CreateInitialSingletonList();
        var username = new Username(PlayerTestData.Username);
        return new Player(username, countries);
    }

    public static Player CreateInitial(int ratingCount)
    {
        var countries = CountryFactory.CreateInitialList(ratingCount);
        var username = new Username(PlayerTestData.Username);
        return new Player(username, countries);
    }

    public static Player CreateWithTotalGamePoints(int totalPoints)
    {
        var player = CreateInitialWithOneRating();
        player.PlayerGameResult.SetTotalPoints(totalPoints);
        return player;
    }

    public static Player CreateWithCompletePlayerGameResult(int totalPoints, int playerGameRank)
    {
        var player = CreateWithTotalGamePoints(totalPoints);
        player.PlayerGameResult.SetRank(new PlayerRank(playerGameRank));
        return player;
    }

    public static Player CreateWithCompleteRatingGameResult(int bonusPoints, int rankDifference)
    {
        var player = CreateInitialWithOneRating();

        var ratingGameResult = player.PlayerRatings[0].RatingGameResult;
        ratingGameResult.SetBonusPoints(new BonusPoints(bonusPoints));
        ratingGameResult.SetRankDifference(rankDifference);
        
        return player;
    }

    public static Player CreateWithTiedRatingsWithPredictionId(int ratingsCount)
    {
        var player = CreateInitial(ratingsCount);
        var ratings = player.PlayerRatings;

        for (int i = 0; i < ratingsCount; i++)
        {
            var prediciton = ratings[i].Prediction;
            prediciton.SetCalculatedRank(new CountryPosition(PredictionTestData.CalculatedRank));
            prediciton.Id = 1+i;
        }

        return player;
    }

    public static Player CreateWithRatedRatingAndCalculatedPrediction()
    {
        var player = CreateInitialWithOneRating();
        
        var rating = player.PlayerRatings[0];
        rating.SetPoints(
            new Points(PlayerRatingTestData.Category1Points),
            new Points(PlayerRatingTestData.Category2Points),
            new Points(PlayerRatingTestData.Category3Points));

        var prediction = rating.Prediction;
        prediction.SetTotalGivenPoints(PredictionTestData.TotalGivenPoints);
        prediction.SetCalculatedRank(new CountryPosition(PredictionTestData.CalculatedRank));
        prediction.SetTieBreakDemotion(new TieBreakDemotion(PredictionTestData.TieBreakDemotion));

        return player;
    }

    public static Player CreateWithRatingsWithSamePoints(int ratingCount)
    {
        var player = CreateInitial(ratingCount);

        for (int i = 0; i < ratingCount; i++)
        {
            player.PlayerRatings[i].SetPoints(
                new Points(PlayerRatingTestData.Category1Points),
                new Points(PlayerRatingTestData.Category2Points),
                new Points(PlayerRatingTestData.Category3Points));
        }

        return player;
    }

    public static Player CreateWithRankedCountries(int ratingCount)
    {
        var player = CreateInitial(ratingCount);

        for (int i = 0; i < ratingCount; i++)
        {
            player.PlayerRatings[i].Country!.SetActualRank(new CountryPosition(i + 1));
        }

        return player;
    }
}
