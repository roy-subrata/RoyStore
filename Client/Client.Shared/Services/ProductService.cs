using System.Net.Http.Json;

namespace Client.Shared.Services;

public class ProductService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");

    public async Task<Pages<GetProductDto>> GetAsync(string search = "", int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync($"api/products?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Pages<GetProductDto>>()
               ?? throw new InvalidOperationException("Failed to retrieve products.");
    }

    public async Task<GetProductDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetProductDto>()
               ?? throw new InvalidOperationException($"Product with id {id} not found.");
    }

    public async Task<GetProductDto> CreateAsync(CreateProductDto product)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetProductDto>()
               ?? throw new InvalidOperationException("Failed to create product.");
    }

    public async Task<GetProductDto> UpdateAsync(string id, UpdateProductDto product)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetProductDto>()
               ?? throw new InvalidOperationException("Failed to update product.");
    }

    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}");
        response.EnsureSuccessStatusCode();
    }
}

// DTOs
public record GetProductDto(
    string Id,
    string Name,
    string LocalName,
    string PartNo,
    EntityRef Brand,
    EntityRef Category,
    EntityRef Unit,
    List<GetProductFeatureDto> Features,
    string Description);

public record GetProductFeatureDto(string Id, string Name, string Value);

public record EntityRef(string Id, string Name);

public record CreateProductDto(
    string Name,
    string LocalName,
    string PartNo,
    string BrandId,
    string CategoryId,
    string UnitId,
    string Description,
    List<CreateFeatureDto> Features);

public record UpdateProductDto(
    string Id,
    string Name,
    string LocalName,
    string PartNo,
    string BrandId,
    string CategoryId,
    string UnitId,
    string Description,
    List<CreateFeatureDto> Features);

public record CreateFeatureDto(string Id, string Value);
