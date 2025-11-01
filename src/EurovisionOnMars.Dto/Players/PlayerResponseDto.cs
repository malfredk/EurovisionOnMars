namespace EurovisionOnMars.Dto.Players;

public record PlayerResponseDto : IdBaseDto
{
    public required string Username { get; set; }
}