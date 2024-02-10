namespace EurovisionOnMars.Dto;

public record RatingDto
    (
    int Id, 
    int? Category1,
    int? Category2,
    int? Category3,
    int PlayerId
    )
{
}
