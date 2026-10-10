using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Povala.Api.Data;
using Povala.Api.Models;
using Povala.Shared.Constants;
using Povala.Shared.DTOs.Auth;
using Povala.Shared.Enums;

namespace Povala.Api.Tests.Integration;

public class PovalaApiFactory : WebApplicationFactory<Program>
{
    public const string TestPassword = "Password123";

    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    // Kept open for the factory's lifetime, because an in-memory SQLite database is deleted when its last connection closes
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureTestServices(services =>
        {
            // Both registrations must go, otherwise EF still applies the original povala.db connection string
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }

    public async Task<(HttpClient Client, int UserId)> CreateClientForNewUserAsync(string role = AppRoles.User)
    {
        var email = $"{Guid.NewGuid():N}@povala.test";
        var userId = await CreateUserAsync(email, role);

        var client = CreateClient();
        var token = await LoginAsync(client, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return (client, userId);
    }

    public Task<int> AddBoulderAsync(BoulderDifficulty difficulty, bool isActive = true)
    {
        return ExecuteDbAsync(async db =>
        {
            var boulder = new Boulder { Difficulty = difficulty, IsActive = isActive };
            db.Boulders.Add(boulder);
            await db.SaveChangesAsync();
            return boulder.Id;
        });
    }

    public Task<int> AddAscentAsync(int userId, int boulderId)
    {
        return ExecuteDbAsync(async db =>
        {
            var ascent = new Ascent { UserId = userId, BoulderId = boulderId };
            db.Ascents.Add(ascent);
            await db.SaveChangesAsync();
            return ascent.Id;
        });
    }

    public Task<bool> AscentExistsAsync(int ascentId)
    {
        return ExecuteDbAsync(db => db.Ascents.AnyAsync(a => a.Id == ascentId));
    }

    private async Task<T> ExecuteDbAsync<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await action(db);
    }

    // Users are created directly instead of via /api/auth/register, because registration can't create admins
    private async Task<int> CreateUserAsync(string email, string role)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User { UserName = email, Email = email, Name = "Test Climber", Role = role };
        var result = await userManager.CreateAsync(user, TestPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return user.Id;
    }

    private static async Task<string> LoginAsync(HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto { Email = email, Password = TestPassword });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }
}
