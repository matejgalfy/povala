using System.Net.Http.Json;
using Povala.Shared.DTOs.Auth;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
    }

    public async Task<bool> RegisterAsync(RegisterDto registerData)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Backend registration failed: {errorMessage}");

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception caught: {ex.Message}");
            return false;
        }
    }
}