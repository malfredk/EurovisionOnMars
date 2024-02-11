namespace EurovisionOnMars.Dto;

public abstract record IdBaseDto
{
    public int Id { get; set; }
    
    protected IdBaseDto(int id)
    {
        Id = id;
    }
}
