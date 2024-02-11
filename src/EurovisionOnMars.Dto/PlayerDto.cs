namespace EurovisionOnMars.Dto;

public record PlayerDto : IdBaseDto
{
    public string Username { get; set; }
    public List<RatingDto>? Ratings { get; set; }

    public PlayerDto(int id, string username, List<RatingDto>? ratings)
        : base(id)
    {
        Username = username;
        Ratings = ratings;
    }
}
