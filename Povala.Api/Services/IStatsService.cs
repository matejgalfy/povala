using Povala.Shared.DTOs.User;

namespace Povala.Api.Services;

public interface IStatsService
{
    Task<UserStatsDto> GetUserStatsAsync(int userId);
}
