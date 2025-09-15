using System.Net.Http.Json;
using System.Text.Json;
using Client.Shared.Services;

public class PurchaseService
{
    private readonly HttpClient _httpClient;

    public PurchaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API"); // Use named client "purchase"
    }

    public async Task<Pages<GetPurchaseDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/purchases?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<GetPurchaseDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve purchases.");
    }

    public async Task<GetPurchaseDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/purchases/{id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetPurchaseDto>();
        return result ?? throw new InvalidOperationException($"Prodcut with id {id} not found.");
    }

    public async Task<CreatePurchaseDto> CreateAsync(CreatePurchaseDto purchase)
    {
        var response = await _httpClient.PostAsJsonAsync("api/purchases", purchase);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CreatePurchaseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<CreatePurchaseDto> UpdateAsync(string id, CreatePurchaseDto purchase)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/purchases/{id}", purchase);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CreatePurchaseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }


    public async Task DeleteAsync(string id)
    {
        await _httpClient.DeleteAsync($"api/purchases/{id}");

    }
}


public enum Status
{
    Draft,
    Pending,
    Done
}
public record CreatePurchaseDto(
    string PurchaseNo,
    string SupplierId,
    DateTime PurchaseDate,
    double DeliveryCharge,
    double Vat,
    double Tax,
    double DiscountAmount,
    double PaidAmount,
    Status Status,
    List<CreatePurchaseItemDto> Items
    );

public record CreatePurchaseItemDto(
    string Id,
    int Quantity,
    double UnitPrice
    );


public record GetPurchaseDto(
    string Id,
    string PurchaseNo,
    string Status,
    DateTime PurchaseDate,
    EntityRef Supplier,
    double SubTotal,
    double TotalAmount,
    double PaidAmount,
    double DueAmount,
    double DeliveryCharge,
    double Vat,
    double Tax,
    double DiscountAmount,
    List<GetPurchaseItemDto> Items
    );

public record GetPurchaseItemDto(
    string Id,
    string ProductName,
    string LocalName,
    string partNo,
    int Quantity,
    int RemainingQuantity,
    double UnitPrice
    );



