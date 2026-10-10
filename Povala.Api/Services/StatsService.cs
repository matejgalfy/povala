using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Shared.DTOs.User;
using Povala.Shared.Enums;

namespace Povala.Api.Services;

public class StatsService : IStatsService
{
    private readonly ApplicationDbContext _context;

    public StatsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserStatsDto> GetUserStatsAsync(int userId)
    {
        var ascendedBoulders = await GetAscendedBouldersAsync(userId);
        var ascendedIds = ascendedBoulders.Select(b => b.Id).ToHashSet();
        var projectBoulderIds = await GetActiveProjectBoulderIdsAsync(userId);

        return new UserStatsDto
        {
            TotalAscents = ascendedBoulders.Count,
            ActiveProjects = projectBoulderIds.Count(id => !ascendedIds.Contains(id)),
            HighestGrade = ascendedBoulders.Count > 0 ? ascendedBoulders.Max(b => b.Difficulty) : null
        };
    }

    // Includes boulders removed from the wall since, so past ascents keep counting
    private async Task<List<(int Id, BoulderDifficulty Difficulty)>> GetAscendedBouldersAsync(int userId)
    {
        var boulders = await _context.Ascents
            .Where(a => a.UserId == userId)
            .Join(_context.Boulders, a => a.BoulderId, b => b.Id, (a, b) => new { b.Id, b.Difficulty })
            .Distinct()
            .ToListAsync();

        return boulders.Select(b => (b.Id, b.Difficulty)).ToList();
    }

    private async Task<List<int>> GetActiveProjectBoulderIdsAsync(int userId)
    {
        return await _context.Projects
            .Where(p => p.UserId == userId)
            .Join(_context.Boulders.Where(b => b.IsActive), p => p.BoulderId, b => b.Id, (p, b) => b.Id)
            .Distinct()
            .ToListAsync();
    }
}
