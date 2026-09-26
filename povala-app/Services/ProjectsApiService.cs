using System.Text;
using System.Text.Json;
using Povala.Shared.DTOs.Project;

namespace povala_app.Services;

public class ProjectsApiService
{
    private readonly HttpClient _httpClient;

    public ProjectsApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PovalaApi");
    }

    public async Task<List<ProjectDto>> GetMyAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("/api/projects/my");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<ProjectDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? [];
    }

    public async Task<ProjectDto?> CreateAsync(CreateProjectDto newProject)
    {
        var json = JsonSerializer.Serialize(newProject);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync("/api/projects", content);
        if (!response.IsSuccessStatusCode)
            return null;
            
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ProjectDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.DeleteAsync($"/api/projects/{id}");
        return response.IsSuccessStatusCode;
    }
}
