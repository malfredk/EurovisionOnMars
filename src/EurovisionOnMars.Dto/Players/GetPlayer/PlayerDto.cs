namespace EurovisionOnMars.Dto.Players.GetPlayer;

public record PlayerDto : IdBaseDto
{
    public required string Username { get; set; }
}