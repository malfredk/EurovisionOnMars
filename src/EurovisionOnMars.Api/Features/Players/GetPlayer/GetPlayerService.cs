using EurovisionOnMars.Dto.Players.GetPlayer;
using EurovisionOnMars.Domain.Players;

namespace EurovisionOnMars.Api.Features.Players.GetPlayer;

public interface IGetPlayerService
{
    Task<PlayerDto> GetPlayer(string username);
}

public class GetPlayerService : IGetPlayerService
{
    private readonly ILogger<GetPlayerService> _logger;
    private readonly IGetPlayerRepository _repository;
    private readonly IPlayerMapper _mapper;

    public GetPlayerService(
        ILogger<GetPlayerService> logger,
        IGetPlayerRepository repository,
        IPlayerMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PlayerDto> GetPlayer(string username)
    {
        var validUsername = new Username(username);

        var player = await _repository.GetPlayer(validUsername);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with username={username} exists.");
        }
        return _mapper.ToDto(player);
    }
}