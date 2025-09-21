using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Shared.Services;

public class PaymentMethodService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API"); // Use named client "PaymentMethod"

    public async Task<Pages<PaymentMethodDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/paymentmethod?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<PaymentMethodDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve PaymentMethods.");
    }

    public async Task<PaymentMethodDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/paymentmethod/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaymentMethodDto>();
        return result ?? throw new InvalidOperationException($"PaymentMethod with id {id} not found.");
    }

    public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethod PaymentMethod)
    {
        var response = await _httpClient.PostAsJsonAsync("api/paymentmethod", PaymentMethod);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaymentMethodDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<PaymentMethodDto> UpdateAsync(string id, UpdatePaymentMethod PaymentMethod)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/paymentmethod/{id}", PaymentMethod);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaymentMethodDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/paymentmethod/{id}");
        response.EnsureSuccessStatusCode();
    }
}



public record PaymentMethodDto(string Id, string ProviderName, string AccountNo, string AccountOwner, bool IsActive); // Changed Id to int

public record CreatePaymentMethod(string ProviderName, string AccountNo, string AccountOwner, bool IsActive);

public record UpdatePaymentMethod(string ProviderName, string AccountNo, string AccountOwner, bool IsActive);