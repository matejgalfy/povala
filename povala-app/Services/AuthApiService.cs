using System.Net.Http.Json;
using Povala.Shared.DTOs.Auth;
using povala_app.Services.Helper;

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
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var errors = System.Text.Json.JsonSerializer.Deserialize<List<IdentityErrorResponse>>(errorContent, options);

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

    public async Task<string?> LoginAsync(LoginDto loginData)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var errors = System.Text.Json.JsonSerializer.Deserialize<List<IdentityErrorResponse>>(errorContent, options);

                if (errors != null && errors.Count > 0)
                {
                    return string.Join("\n", errors.Select(e => e.Description));
                }
            }
            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                return errorContent;
            }
            
            return "Login failed.";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}

