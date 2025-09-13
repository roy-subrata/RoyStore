using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Shared.Services;

public class UnitService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Brand"

    public async Task<Pages<UnitDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/units?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<UnitDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve units.");
    }

    public async Task<UnitDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/units/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<UnitDto>();
        return result ?? throw new InvalidOperationException($"Brand with id {id} not found.");
    }

    public async Task<UnitDto> CreateAsync(CreateUnitDto Brand)
    {
        var response = await _httpClient.PostAsJsonAsync("api/units", Brand);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UnitDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<UnitDto> UpdateAsync(string id, UpdateUnitDto Brand)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/units/{id}", Brand);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UnitDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/units/{id}");
        response.EnsureSuccessStatusCode();
    }
}



public record UnitDto(string Id, string Name,string ShortCode, bool IsBaseUnit); // Changed Id to int

public record CreateUnitDto(string Name, string ShortCode,bool IsBaseUnit);

public record UpdateUnitDto(string Id, string Name, string ShortCode, bool IsBaseUnit);