using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;

namespace EurovisionOnMars.Api.Repositories;

public interface IRatingResultRepository
{
    Task<RatingResult> UpdateRatingResult(RatingResult ratingResult);
}

public class RatingResultRepository : IRatingResultRepository
{
    private readonly DataContext _context;
    private readonly ILogger<RatingResultRepository> _logger;

    public RatingResultRepository(DataContext context, ILogger<RatingResultRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RatingResult> UpdateRatingResult(RatingResult ratingResult)
    {
        _logger.LogDebug($"Updating ratingResult with id={ratingResult.Id}");
        var updatedRatingResult = _context.Update(ratingResult);
        await _context.SaveChangesAsync();
        return updatedRatingResult.Entity;
    }
}