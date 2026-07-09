using System.Net.Http.Json;
using Povala.Shared.DTOs.Auth;
using System.Diagnostics;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
    }

    public async Task<bool> RegisterAsync(RegisterDto registerData)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerData);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception($"Status: {response.StatusCode}, Body: {errorMessage}");
    }
}