namespace EurovisionOnMars.Dto;

public record PlayerDto(int Id, string Username, List<RatingDto>? Ratings);
