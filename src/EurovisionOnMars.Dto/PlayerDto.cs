namespace EurovisionOnMars.Dto;

public record PlayerDto : IdBaseDto
{
    public required string Username { get; set; }
    public List<RatingDto>? Ratings { get; set; }
}
