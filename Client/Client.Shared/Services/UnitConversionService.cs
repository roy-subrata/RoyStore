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
        var response = await _httpClient.GetAsync($"api/unitConversion?search={search}&page={page}&pageSize={pageSize}");
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

    public async Task<UnitConversionDto> CreateAsync(CreateUnitConversionDto unitconversion)
    {
        var response = await _httpClient.PostAsJsonAsync("api/unitconversion", unitconversion);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UnitConversionDto>()
                       ?? throw new InvalidOperationException("Failed to create Unit Conversion.");

    }

    public async Task<UnitConversionDto> UpdateAsync(string id, UpdateUnitConversionDto unitConversion)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/unitconversion/{id}", unitConversion);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UnitConversionDto>()
                              ?? throw new InvalidOperationException("Failed to update Unit Conversion.");

    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/unitconversion/{id}");
        response.EnsureSuccessStatusCode();
    }
}



public record UnitConversionDto(string Id, EntityRef FromUnit, EntityRef ToUnit, double Factor); // Changed Id to int

public record CreateUnitConversionDto(string FromUnitId, string ToUnitId, double Factor);

public record UpdateUnitConversionDto(string Id, string FromUnitId, string ToUnitId, double Factor);