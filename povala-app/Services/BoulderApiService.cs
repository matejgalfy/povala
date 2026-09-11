using System.Text;
using System.Text.Json;
using Povala.Shared.DTOs.Boulder;

namespace povala_app.Services;

public class BoulderApiService
{
    private readonly HttpClient _httpClient;

    public BoulderApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
    }

    public async Task<List<BoulderDto>> GetAllBouldersAsync()
    {
        var boulders = await _httpClient.GetAsync("/api/boulders");

        boulders.EnsureSuccessStatusCode();
        var json = await boulders.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<BoulderDto>>(json);
        return result ?? [];
    }

    public async Task<BoulderDto?> CreateAsync(CreateBoulderDto newBoulder)
    {
        var json = JsonSerializer.Serialize(newBoulder);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/boulders", content);
        if (!response.IsSuccessStatusCode)
            return null;
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BoulderDto>(responseJson);
    }



}

