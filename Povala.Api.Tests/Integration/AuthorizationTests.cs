using System.Net;
using System.Net.Http.Json;
using Povala.Shared.Constants;
using Povala.Shared.DTOs.Boulder;
using Povala.Shared.Enums;

namespace Povala.Api.Tests.Integration;

public class AuthorizationTests : IClassFixture<PovalaApiFactory>
{
    private static readonly CreateBoulderDto NewBoulder = new()
    {
        Difficulty = BoulderDifficulty.V3,
        Color = BoulderColor.Red,
        CoordX = 100,
        CoordY = 200
    };

    private readonly PovalaApiFactory _factory;

    public AuthorizationTests(PovalaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateBoulder_AsRegularUser_ReturnsForbidden()
    {
        var (client, _) = await _factory.CreateClientForNewUserAsync();

        var response = await client.PostAsJsonAsync("/api/boulders", NewBoulder, PovalaApiFactory.JsonOptions);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateBoulder_AsAdmin_ReturnsOk()
    {
        var (client, _) = await _factory.CreateClientForNewUserAsync(AppRoles.Admin);

        var response = await client.PostAsJsonAsync("/api/boulders", NewBoulder, PovalaApiFactory.JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAscent_OfAnotherUser_ReturnsForbiddenAndKeepsAscent()
    {
        var (_, ownerId) = await _factory.CreateClientForNewUserAsync();
        var (otherClient, _) = await _factory.CreateClientForNewUserAsync();
        var boulderId = await _factory.AddBoulderAsync(BoulderDifficulty.V2);
        var ascentId = await _factory.AddAscentAsync(ownerId, boulderId);

        var response = await otherClient.DeleteAsync($"/api/ascents/{ascentId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(await _factory.AscentExistsAsync(ascentId));
    }

    [Fact]
    public async Task DeleteAscent_OwnAscent_ReturnsNoContentAndRemovesAscent()
    {
        var (client, userId) = await _factory.CreateClientForNewUserAsync();
        var boulderId = await _factory.AddBoulderAsync(BoulderDifficulty.V2);
        var ascentId = await _factory.AddAscentAsync(userId, boulderId);

        var response = await client.DeleteAsync($"/api/ascents/{ascentId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(await _factory.AscentExistsAsync(ascentId));
    }
}
