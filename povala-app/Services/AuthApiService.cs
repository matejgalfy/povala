using System.Net.Http.Json;
using Povala.Shared.DTOs.Auth;
using System.Diagnostics;
using povala_app.Services.Helper;

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

            var errorJson = await response.Content.ReadAsStringAsync();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<IdentityErrorResponse>>(errorJson, options);
            if (errors != null && errors.Count > 0)
            {
                return string.Join("\n", errors.Select(e => e.Description));
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}

