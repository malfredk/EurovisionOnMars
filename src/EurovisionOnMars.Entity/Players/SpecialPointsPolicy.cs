using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Players;

internal static class SpecialPointsPolicy
{
    internal static void Validate(
        int ratingIdToUpdate,
        List<PlayerRating> ratings,
        Points category1Points,
        Points category2Points,
        Points category3Points)
    {
        Func<PlayerRating, Points?> category1PointsGetter = r => r.Category1Points;
        ValidateSpecialCategoryPoints(ratingIdToUpdate, ratings, category1Points, category1PointsGetter);

        Func<PlayerRating, Points?> category2PointsGetter = r => r.Category2Points;
        ValidateSpecialCategoryPoints(ratingIdToUpdate, ratings, category2Points, category2PointsGetter);

        Func<PlayerRating, Points?> category3PointsGetter = r => r.Category3Points;
        ValidateSpecialCategoryPoints(ratingIdToUpdate, ratings, category3Points, category3PointsGetter);
    }

    private static void ValidateSpecialCategoryPoints(
        int ratingIdToUpdate,
        List<PlayerRating> ratings,
        Points newPoints,
        Func<PlayerRating, Points?> pointsSelector
        )
    {
        if (!newPoints.IsSpecial)
        {
            return;
        }

        EnsureUniqueSpecialPoints(ratingIdToUpdate, ratings, newPoints, pointsSelector);
    }

    private static void EnsureUniqueSpecialPoints(
        int ratingIdToUpdate,
        List<PlayerRating> ratings,
        Points newPoints,
        Func<PlayerRating, Points?> pointsSelector
        )
    {
        var hasGivenPointsToOtherCountry = ratings
            .AsReadOnly()
            .Where(r => r.Id != ratingIdToUpdate)
            .Any(r => pointsSelector(r) == newPoints);

        if (hasGivenPointsToOtherCountry)
        {
            throw new ArgumentException("These special points have already been given in this category to another country.");
        }
    }
}
