using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Shared.Services;

public class PurchaseService
{
    private readonly HttpClient _httpClient;

    public PurchaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API");
    }

    public async Task<Pages<GetPurchaseDto>> GetAsync(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _httpClient.GetAsync($"api/purchase?search={search}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Pages<GetPurchaseDto>>();
        return result ?? throw new InvalidOperationException("Failed to retrieve purchases.");
    }

    public async Task<GetPurchaseDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/purchase/{id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetPurchaseDto>();
        return result ?? throw new InvalidOperationException($"Prodcut with id {id} not found.");
    }

    public async Task<CreatePurchaseDto> CreateAsync(CreatePurchaseDto purchase)
    {
        var response = await _httpClient.PostAsJsonAsync("api/purchase", purchase);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CreatePurchaseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<CreatePurchaseDto> UpdateAsync(string id, CreatePurchaseDto purchase)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/purchase/{id}", purchase);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CreatePurchaseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task DeleteAsync(string id)
    {
        await _httpClient.DeleteAsync($"api/purchase/{id}");

    }
}

public record CreatePurchaseDto(
    string PurchaseNo,
    DateTime PurchaseDate,
    string SupplierId,
    PurchaseStatus Status,
    double Vat,
    double Tax,
    double DiscountAmount,
    string NoteRef,
    List<CreatePurchaseItemDto> Items
    );

public record CreatePurchaseItemDto(
    string Id,
    string UnitId,
    double Quantity,
    double UnitPrice
    );

public enum PurchaseStatus
{
    [Description("Purchase created but not confirmed")]
    Draft,

    [Description("Purchase order sent to supplier")]
    Ordered,

    [Description("Some items received, some pending")]
    PartialReceived,

    [Description("All items received from supplier")]
    Received,

    [Description("Items verified for quality")]
    QualityCheck,

    [Description("Items approved and moved to stock")]
    ReadyToStock,

    [Description("Purchase order cancelled")]
    Cancelled
}
public record GetPurchaseDto(
    string Id,
    string PurchaseNo,
    string Status,
    DateTime PurchaseDate,
    EntityRef Supplier,
    double Vat,
    double Tax,
    double DiscountAmount,
    double SubTotal,
    double Paid,
    double Due,
    List<GetPurchaseItemDto> Items
    );

public record GetPurchaseItemDto(
    string Id,
    string ProductName,
    string LocalName,
    string partNo,
    double Quantity,
    double OrderedQuantity,
    double UnitPrice
    );



