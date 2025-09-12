
using System.Net.Http.Json;
using System.Text.Json;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Category"
    }

    public async Task<Pages<CategoryDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/categories?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<CategoryDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve categories.");
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/categories/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CategoryDto>();
        return result ?? throw new InvalidOperationException($"Category with id {id} not found.");
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto category)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", category);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CategoryDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<CategoryDto> UpdateAsync(string id, UpdateCategoryDto category)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", category);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CategoryDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}");
        response.EnsureSuccessStatusCode();
    }
}

public class Pages<T>
{
    public int page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public List<T> Data { get; set; } = new();
}

public record CategoryDto(string Id, string Name, string Description, string ParentId, List<CategoryDto> Children); // Changed Id to int

public record CreateCategoryDto(string Name, string Description, string ParentId);

public record UpdateCategoryDto(string Name, string Description, string ParentId);