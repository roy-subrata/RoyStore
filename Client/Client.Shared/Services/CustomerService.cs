using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Shared.Services;

public class CustomerService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API"); // Use named client "Customer"

    public async Task<Pages<CustomerDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/customers?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<CustomerDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve customers.");
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/customers/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        return result ?? throw new InvalidOperationException($"Customer with id {id} not found.");
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto Customer)
    {
        var response = await _httpClient.PostAsJsonAsync("api/customers", Customer);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CustomerDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<CustomerDto> UpdateAsync(string id, UpdateCustomerDto Customer)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/customers/{id}", Customer);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CustomerDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/customers/{id}");
        response.EnsureSuccessStatusCode();
    }
}


public record CustomerDto(string Id, string Name, string Phone, string Email,string Address); // Changed Id to int

public record CreateCustomerDto(string Name, string Phone,  string Email, string Address);

public record UpdateCustomerDto(string Name, string Phone, string Email, string Address);