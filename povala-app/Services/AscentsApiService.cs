using System.Text;
using System.Text.Json;
using Povala.Shared.DTOs.Ascent;

namespace povala_app.Services;

public class AscentsApiService
{
    private readonly HttpClient _httpClient;

    public AscentsApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
    }

    public async Task<List<AscentDto>> GetMyAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("/api/ascents/my");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<AscentDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? [];
    }

    public async Task<AscentDto?> CreateAsync(CreateAscentDto newAscent)
    {
        var json = JsonSerializer.Serialize(newAscent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync("/api/ascents", content);
        if (!response.IsSuccessStatusCode)
            return null;
            
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AscentDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.DeleteAsync($"/api/ascents/{id}");
        return response.IsSuccessStatusCode;
    }
}
