using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Api.Models;
using Povala.Api.Services;
using Povala.Shared.Enums;

namespace Povala.Api.Tests.Services;

public class StatsServiceTests : IDisposable
{
    private const int UserId = 1;
    private const int OtherUserId = 2;

    // An in-memory SQLite database only lives while its connection is open, so each test gets a fresh one
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly StatsService _service;

    public StatsServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
        _service = new StatsService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetUserStats_WithNoActivity_ReturnsZeroesAndNoGrade()
    {
        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(0, stats.TotalAscents);
        Assert.Equal(0, stats.ActiveProjects);
        Assert.Null(stats.HighestGrade);
    }

    [Fact]
    public async Task GetUserStats_CountsAscentsOnRemovedBoulders()
    {
        var activeBoulder = await AddBoulderAsync(BoulderDifficulty.V2);
        var removedBoulder = await AddBoulderAsync(BoulderDifficulty.V3, isActive: false);
        await AddAscentAsync(UserId, activeBoulder);
        await AddAscentAsync(UserId, removedBoulder);

        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(2, stats.TotalAscents);
    }

    [Fact]
    public async Task GetUserStats_CountsDuplicateAscentOnce()
    {
        var boulder = await AddBoulderAsync(BoulderDifficulty.V2);
        await AddAscentAsync(UserId, boulder);
        await AddAscentAsync(UserId, boulder);

        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(1, stats.TotalAscents);
    }

    [Fact]
    public async Task GetUserStats_ReturnsHighestAscendedGradeIncludingRemovedBoulders()
    {
        await AddAscentAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V2));
        await AddAscentAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V6, isActive: false));
        await AddAscentAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V4));
        await AddProjectAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V9));

        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(BoulderDifficulty.V6, stats.HighestGrade);
    }

    [Fact]
    public async Task GetUserStats_IgnoresProjectsOnRemovedBoulders()
    {
        await AddProjectAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V5));
        await AddProjectAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V6, isActive: false));

        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(1, stats.ActiveProjects);
    }

    [Fact]
    public async Task GetUserStats_IgnoresProjectsAlreadyAscended()
    {
        var sentProject = await AddBoulderAsync(BoulderDifficulty.V5);
        await AddProjectAsync(UserId, sentProject);
        await AddAscentAsync(UserId, sentProject);
        await AddProjectAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V7));

        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(1, stats.ActiveProjects);
    }

    [Fact]
    public async Task GetUserStats_IgnoresOtherUsersActivity()
    {
        await AddAscentAsync(OtherUserId, await AddBoulderAsync(BoulderDifficulty.V8));
        await AddProjectAsync(OtherUserId, await AddBoulderAsync(BoulderDifficulty.V9));
        await AddAscentAsync(UserId, await AddBoulderAsync(BoulderDifficulty.V1));

        var stats = await _service.GetUserStatsAsync(UserId);

        Assert.Equal(1, stats.TotalAscents);
        Assert.Equal(0, stats.ActiveProjects);
        Assert.Equal(BoulderDifficulty.V1, stats.HighestGrade);
    }

    private async Task<int> AddBoulderAsync(BoulderDifficulty difficulty, bool isActive = true)
    {
        var boulder = new Boulder { Difficulty = difficulty, IsActive = isActive };
        _context.Boulders.Add(boulder);
        await _context.SaveChangesAsync();
        return boulder.Id;
    }

    private async Task AddAscentAsync(int userId, int boulderId)
    {
        _context.Ascents.Add(new Ascent { UserId = userId, BoulderId = boulderId });
        await _context.SaveChangesAsync();
    }

    private async Task AddProjectAsync(int userId, int boulderId)
    {
        _context.Projects.Add(new Project { UserId = userId, BoulderId = boulderId });
        await _context.SaveChangesAsync();
    }
}
