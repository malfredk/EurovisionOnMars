using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultService
{
    Task<ImmutableList<PlayerGameResult>> GetPlayerGameResults();
    Task CalculatePlayerGameResults();
}

public class PlayerGameResultService : IPlayerGameResultService
{
    private readonly IPlayerGameResultRepository _playerGameResultRepository;
    private readonly ILogger<PlayerGameResultService> _logger;
    private readonly IRatingGameResultService _ratingGameResultService;

    public PlayerGameResultService
        (
        IPlayerGameResultRepository playerGameResultRepository,
        ILogger<PlayerGameResultService> logger,
        IRatingGameResultService ratingGameResultService
        )
    {
        _playerGameResultRepository = playerGameResultRepository;
        _logger = logger;
        _ratingGameResultService = ratingGameResultService;
    }

    public async Task<ImmutableList<PlayerGameResult>> GetPlayerGameResults()
    {
        var playerGameResults = await _playerGameResultRepository.GetPlayerGameResults();
        return playerGameResults
            .OrderBy(p => p.Rank ?? int.MaxValue)
            .ToImmutableList();
    }

    public async Task CalculatePlayerGameResults()
    {
        var playerGameResults = await _playerGameResultRepository.GetPlayerGameResults();
        await CalculateTotalPoints(playerGameResults);
        CalculateRanks(playerGameResults);
        SavePlayerGameResults(playerGameResults);
    }

    private async Task CalculateTotalPoints(IReadOnlyList<PlayerGameResult> playerGameResults)
    {
        foreach (var playerGameResult in playerGameResults)
        {
            await CalculateTotalPoints(playerGameResult);
        }
    }

    internal async Task CalculateTotalPoints(PlayerGameResult playerGameResult)
    {
        var ratingGameResults = await _ratingGameResultService
            .GetRatingGameResults(playerGameResult.PlayerId);

        var totalPoints = SumBonusPoints(ratingGameResults) + SumRankDifferences(ratingGameResults);
        playerGameResult.SetTotalPoints(totalPoints);
    }

    private int SumBonusPoints(ImmutableList<RatingGameResult> ratingGameResults)
    {
        return ratingGameResults
            .Sum(r => r.BonusPoints ?? throw new Exception("Missing bonus points"));
    }

    private int SumRankDifferences(ImmutableList<RatingGameResult> ratingGameResults)
    {
        return ratingGameResults
            .Sum(r => Math.Abs(r.RankDifference ?? throw new Exception("Missing ranking difference")));
    }

    internal void CalculateRanks(IReadOnlyList<PlayerGameResult> playerGameResults)
    {
        var orderedPlayerGameResults = playerGameResults
            .OrderBy(p => p.TotalPoints)
            .ToList();

        PlayerGameResult? previous = null;
        for (int i = 0; i < orderedPlayerGameResults.Count; i++)
        {
            var current = orderedPlayerGameResults[i];
            if (previous != null && current.TotalPoints == previous.TotalPoints)
            {
                current.SetRank((int)previous.Rank);
            }
            else
            {
                current.SetRank(i+1);
            }
            previous = current;
        }
    }

    private void SavePlayerGameResults(IReadOnlyList<PlayerGameResult> playerGameResults)
    {
        foreach (var playerGameResult in playerGameResults)
        {
            _playerGameResultRepository.UpdatePlayerGameResult(playerGameResult);
        }
    }
}