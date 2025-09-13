using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Shared.Services;

public class UnitConversionService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Brand"

    public async Task<Pages<UnitConversionDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/unitconversion?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<UnitConversionDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve unitconversion.");
    }

    public async Task<UnitConversionDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/unitconversion/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<UnitConversionDto>();
        return result ?? throw new InvalidOperationException($"Brand with id {id} not found.");
    }

    public async Task<UnitConversionDto> CreateAsync(CreateUnitConversionDto Brand)
    {
        var response = await _httpClient.PostAsJsonAsync("api/unitconversion", Brand);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UnitConversionDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<UnitConversionDto> UpdateAsync(string id, UpdateUnitConversionDto Brand)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/unitconversion/{id}", Brand);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UnitConversionDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/unitconversion/{id}");
        response.EnsureSuccessStatusCode();
    }
}



public record UnitConversionDto(string Id, EntityRef FromUnit, EntityRef ToUnit, double Factor); // Changed Id to int

public record CreateUnitConversionDto(string FromUnit, string ToUnit, double Factor);

public record UpdateUnitConversionDto(string Id, string FromUnit, string ToUnit, double Factor);