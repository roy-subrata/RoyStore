
using System.Net.Http.Json;
using System.Text.Json;

public class SupplierService
{
    private readonly HttpClient _httpClient;

    public SupplierService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Supplier"
    }

    public async Task<Pages<SupplierDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/suppliers?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<SupplierDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve suppliers.");
    }

    public async Task<SupplierDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/suppliers/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SupplierDto>();
        return result ?? throw new InvalidOperationException($"Supplier with id {id} not found.");
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto Supplier)
    {
        var response = await _httpClient.PostAsJsonAsync("api/suppliers", Supplier);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SupplierDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<SupplierDto> UpdateAsync(string id, UpdateSupplierDto Supplier)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/suppliers/{id}", Supplier);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SupplierDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/suppliers/{id}");
        response.EnsureSuccessStatusCode();
    }
}


public record SupplierDto(string Id, string Name, string Phone, string Email, string Address); // Changed Id to int

public record CreateSupplierDto(string Name, string Email, string Phone, string Address);

public record UpdateSupplierDto(string Name, string Phone, string Email, string Address);