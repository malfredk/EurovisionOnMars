using EurovisionOnMars.Domain.Players.PlayerRatings;

namespace EurovisionOnMars.Domain.Players;

internal static class SpecialPointsPolicy
{
    internal static void Validate(
        PlayerRating ratingToUpdate,
        IReadOnlyCollection<PlayerRating> ratings,
        Points category1Points,
        Points category2Points,
        Points category3Points)
    {
        Func<PlayerRating, Points?> category1PointsGetter = r => r.Category1Points;
        ValidateSpecialCategoryPoints(ratingToUpdate, ratings, category1Points, category1PointsGetter);

        Func<PlayerRating, Points?> category2PointsGetter = r => r.Category2Points;
        ValidateSpecialCategoryPoints(ratingToUpdate, ratings, category2Points, category2PointsGetter);

        Func<PlayerRating, Points?> category3PointsGetter = r => r.Category3Points;
        ValidateSpecialCategoryPoints(ratingToUpdate, ratings, category3Points, category3PointsGetter);
    }

    private static void ValidateSpecialCategoryPoints(
        PlayerRating ratingToUpdate,
        IReadOnlyCollection<PlayerRating> ratings,
        Points newPoints,
        Func<PlayerRating, Points?> pointsSelector
        )
    {
        if (!newPoints.IsSpecial)
        {
            return;
        }

        EnsureUniqueSpecialPoints(ratingToUpdate, ratings, newPoints, pointsSelector);
    }

    private static void EnsureUniqueSpecialPoints(
        PlayerRating ratingToUpdate,
        IReadOnlyCollection<PlayerRating> ratings,
        Points newPoints,
        Func<PlayerRating, Points?> pointsSelector
        )
    {
        var hasGivenThesePointsToOtherCountry = ratings
            .Where(r => !ReferenceEquals(ratingToUpdate, r))
            .Any(r => pointsSelector(r) == newPoints);

        if (hasGivenThesePointsToOtherCountry)
        {
            throw new ArgumentException("These special points have already been given in this category to another country.");
        }
    }
}
