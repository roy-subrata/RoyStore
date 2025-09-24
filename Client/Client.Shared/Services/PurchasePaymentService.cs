using System.Net.Http.Json;
namespace Client.Shared.Services;

public class PurchasePaymentService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");

    public async Task<Pages<GetPurchasePaymentDto>?> GetAsync(
        string search = "",
        string purchaseId = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync(
            $"api/purchase/payment?purchaseId={purchaseId}&search={search}&page={page}&pageSize={pageSize}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<GetPurchasePaymentDto>>();
        return result;
    }

    public async Task<GetPurchasePaymentDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/purchase/payment/{id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetPurchasePaymentDto>();
        return result ?? throw new InvalidOperationException($"Product with id {id} not found.");
    }

    public async Task<CreatePurchasePaymentDto> CreateAsync(CreatePurchasePaymentDto purchase)
    {
        var response = await _httpClient.PostAsJsonAsync("api/purchase/payment", purchase);
        //response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return purchase;
    }

    public async Task<CreatePurchasePaymentDto> UpdateAsync(string id, CreatePurchasePaymentDto purchase)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/purchase/payment/{id}", purchase);
       // response.EnsureSuccessStatusCode();

       // var content = await response.Content.ReadAsStringAsync();
        return purchase;
    }

    public async Task DeleteAsync(string id)
    {
        await _httpClient.DeleteAsync($"api/purchase/payment/{id}");

    }
}

public record CreatePurchasePaymentDto(
    string PurchaseId,
    string SupplierId,
    string PaymentMethodId,
    DateTime PaymentDate,
    double PaymentAmount,
    string NoteRef
);

public record GetPurchasePaymentDto(
    string Id,
    string PurchaseId,
    DateTime PaymentDate,
    string SupplierId,
    string PaymentMethodId,
    double PaymentAmount,
    string NoteRef
);