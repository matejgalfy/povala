using Povala.Shared.Enums;

namespace Povala.Shared.DTOs.User;

public class UserStatsDto
{
    public int TotalAscents { get; set; }
    public int ActiveProjects { get; set; }
    public BoulderDifficulty? HighestGrade { get; set; }
}
