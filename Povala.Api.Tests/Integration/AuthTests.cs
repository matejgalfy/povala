using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Povala.Shared.DTOs.Auth;

namespace Povala.Api.Tests.Integration;

public class AuthTests : IClassFixture<PovalaApiFactory>
{
    private readonly PovalaApiFactory _factory;

    public AuthTests(PovalaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ThenLogin_ReturnsToken()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@povala.test";

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto { Email = email, Password = PovalaApiFactory.TestPassword, Name = "New Climber" });
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = email, Password = PovalaApiFactory.TestPassword });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var body = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrEmpty(body.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Register_WithTooShortPassword_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto { Email = $"{Guid.NewGuid():N}@povala.test", Password = "short", Name = "New Climber" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@povala.test";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto { Email = email, Password = PovalaApiFactory.TestPassword, Name = "New Climber" });

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = email, Password = "WrongPassword123" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
