﻿namespace EurovisionOnMars.Dto.PlayerGameResults;

public record PlayerGameResultResponseDto : IdBaseDto
{
    public int? Rank { get; set; }
    public int? TotalPoints { get; set; }
    public required string PlayerUsername { get; set; }
}