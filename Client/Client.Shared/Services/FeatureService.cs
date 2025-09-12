using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Shared.Services;

public class FeatureService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Feature"

    public async Task<Pages<FeatureDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/features?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<FeatureDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve Features.");
    }

    public async Task<FeatureDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/features/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FeatureDto>();
        return result ?? throw new InvalidOperationException($"Feature with id {id} not found.");
    }

    public async Task<FeatureDto> CreateAsync(CreateFeature feature)
    {
        var response = await _httpClient.PostAsJsonAsync("api/features", feature);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FeatureDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<FeatureDto> UpdateAsync(string id, UpdateFeature feature)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/features/{id}", feature);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FeatureDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/Features/{id}");
        response.EnsureSuccessStatusCode();
    }
}



public record FeatureDto(string Id, string Name, bool IsActive); // Changed Id to int

public record CreateFeature(string Name, bool IsActive);

public record UpdateFeature(string Id,string Name,  bool IsActive);