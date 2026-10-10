using System.Net;
using System.Net.Http.Json;
using Povala.Shared.DTOs.Ascent;
using Povala.Shared.DTOs.Project;
using Povala.Shared.DTOs.User;
using Povala.Shared.Enums;

namespace Povala.Api.Tests.Integration;

public class StatsEndpointTests : IClassFixture<PovalaApiFactory>
{
    private readonly PovalaApiFactory _factory;

    public StatsEndpointTests(PovalaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMyStats_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/stats/my");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyStats_ReflectsAscentsAndProjectsMarkedThroughApi()
    {
        var (client, _) = await _factory.CreateClientForNewUserAsync();
        var climbedBoulder = await _factory.AddBoulderAsync(BoulderDifficulty.V4);
        var projectBoulder = await _factory.AddBoulderAsync(BoulderDifficulty.V6);

        (await client.PostAsJsonAsync("/api/ascents", new CreateAscentDto { BoulderId = climbedBoulder })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync("/api/projects", new CreateProjectDto { BoulderId = projectBoulder })).EnsureSuccessStatusCode();

        var stats = await client.GetFromJsonAsync<UserStatsDto>("/api/stats/my", PovalaApiFactory.JsonOptions);

        Assert.NotNull(stats);
        Assert.Equal(1, stats.TotalAscents);
        Assert.Equal(1, stats.ActiveProjects);
        Assert.Equal(BoulderDifficulty.V4, stats.HighestGrade);
    }
}
