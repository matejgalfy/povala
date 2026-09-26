using Povala.Shared.DTOs.Auth;
using povala_app.Services.Helper;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace povala_app.Services;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
    }

    public async Task<string?> RegisterAsync(RegisterDto registerData)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var errors = JsonSerializer.Deserialize<List<IdentityErrorResponse>>(errorContent, options);

                if (errors != null && errors.Count > 0)
                {
                    return string.Join("\n", errors.Select(e => e.Description));
                }
            }
            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                return errorContent;
            }

            return "Registration failed.";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<(string? Error, string? Role)> LoginAsync(LoginDto loginData)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                string? token = null;
                if (json.TryGetProperty("token", out var tokenProp)) token = tokenProp.GetString();
                else if (json.TryGetProperty("Token", out var tokenProp2)) token = tokenProp2.GetString();

                if (string.IsNullOrEmpty(token))
                    return ("Login failed. Token not received.", null);

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                
                var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value ?? "User";

                await SecureStorage.SetAsync("auth_token", token);
                await SecureStorage.SetAsync("user_role", role);

                return (null, role);
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var errors = JsonSerializer.Deserialize<List<IdentityErrorResponse>>(errorContent, options);

                if (errors != null && errors.Count > 0)
                {
                    return (string.Join("\n", errors.Select(e => e.Description)), null);
                }
            }
            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                return (errorContent, null);
            }
            
            return ("Login failed.", null);
        }
        catch (Exception ex)
        {
            return (ex.Message, null);
        }
    }
}

