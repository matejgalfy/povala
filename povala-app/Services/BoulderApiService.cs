using System.Text;
using System.Text.Json;
using Povala.Shared.DTOs.Boulder;

namespace povala_app.Services;

public class BoulderApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public BoulderApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }

    public async Task<List<BoulderDto>> GetAllBouldersAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var boulders = await _httpClient.GetAsync("/api/boulders");

        boulders.EnsureSuccessStatusCode();
        var json = await boulders.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<BoulderDto>>(json, _jsonOptions);
        return result ?? [];
    }

    public async Task<BoulderDto?> CreateAsync(CreateBoulderDto newBoulder)
    {
        var json = JsonSerializer.Serialize(newBoulder, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync("/api/boulders", content);
        if (!response.IsSuccessStatusCode)
            return null;
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BoulderDto>(responseJson, _jsonOptions);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.DeleteAsync($"/api/boulders/{id}");
        return response.IsSuccessStatusCode;
    }
}
