namespace EurovisionOnMars.Dto;

public record PlayerDto : IdBaseDto
{
    public required string Username { get; set; }
    public List<PlayerRatingDto>? PlayerRatings { get; set; }
    public PlayerGameResultDto? PlayerGameResult { get; set; }
}