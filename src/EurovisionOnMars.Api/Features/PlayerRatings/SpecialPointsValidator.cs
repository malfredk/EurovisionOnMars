using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface ISpecialPointsValidator
{
    public void ValidateSpecialCategoryPoints(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings);
}

public class SpecialPointsValidator : ISpecialPointsValidator
{
    private readonly ILogger<SpecialPointsValidator> _logger;

    public SpecialPointsValidator(ILogger<SpecialPointsValidator> logger)
    {
        _logger = logger;
    }

    public void ValidateSpecialCategoryPoints(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings) // TODO: test
    {
        Func<PlayerRating, int?> category1PointsGetter = r => r.Category1Points;
        Func<PlayerRating, int?> category2PointsGetter = r => r.Category2Points;
        Func<PlayerRating, int?> category3PointsGetter = r => r.Category3Points;

        _logger.LogDebug("Validating points in rating for category 1.");
        ValidateSpecialCategoryPoints(editedRating, ratings, category1PointsGetter);

        _logger.LogDebug("Validating points in rating for category 2.");
        ValidateSpecialCategoryPoints(editedRating, ratings, category2PointsGetter);

        _logger.LogDebug("Validating points in rating for category 3.");
        ValidateSpecialCategoryPoints(editedRating, ratings, category3PointsGetter);
    }

    private void ValidateSpecialCategoryPoints(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratings,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        var points = (int)categoryPointsGetter(rating)!;
        if (!PlayerRating.SPECIAL_POINTS.Contains(points))
        {
            _logger.LogDebug("Skipping validation since edited rating does not have special points in this category.");
            return;
        }
        EnsureUniqueSpecialPoints(points, ratings, categoryPointsGetter);
    }

    private void EnsureUniqueSpecialPoints(
        int specialPoints,
        IReadOnlyList<PlayerRating> ratings,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        var specialPointsCount = ratings
            .Count(r => categoryPointsGetter(r) == specialPoints);
        if (specialPointsCount > 1)
        {
            throw new ArgumentException("Special points have already been given in category.");
        }
    }
}
