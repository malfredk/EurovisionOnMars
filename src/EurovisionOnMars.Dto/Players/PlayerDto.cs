namespace EurovisionOnMars.Dto.Players;

public record PlayerDto : IdBaseDto
{
    public required string Username { get; set; }
}