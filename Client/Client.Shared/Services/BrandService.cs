using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Shared.Services;

public class BrandService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Brand"

    public async Task<Pages<BrandDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/brands?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<BrandDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve brands.");
    }

    public async Task<BrandDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/brands/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BrandDto>();
        return result ?? throw new InvalidOperationException($"Brand with id {id} not found.");
    }

    public async Task<BrandDto> CreateAsync(CreateBrandDto Brand)
    {
        var response = await _httpClient.PostAsJsonAsync("api/brands", Brand);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BrandDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<BrandDto> UpdateAsync(string id, UpdateBrandDto Brand)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/brands/{id}", Brand);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BrandDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/brands/{id}");
        response.EnsureSuccessStatusCode();
    }
}



public record BrandDto(string Id, string Name, string Description); // Changed Id to int

public record CreateBrandDto(string Name, string Description);

public record UpdateBrandDto(string Name, string Description);