namespace EurovisionOnMars.Dto;

public record RatingDto : IdBaseDto
{
    public int? Category1 { get; set; }
    public int? Category2 { get; set; }
    public int? Category3 { get; set; }
    public int PlayerId { get; set; }
    
    public RatingDto
        (
        int id, 
        int? category1, 
        int? category2,
        int? category3,
        int playerId)
        : base(id)
    {
        Category1 = category1;
        Category2 = category2;
        Category3 = category3;
        PlayerId = playerId;
    }
}
