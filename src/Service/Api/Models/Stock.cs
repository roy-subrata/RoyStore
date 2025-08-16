

namespace Api.Models;

public record GetStock
    (string Id, string PurchaseNo, EntityRef Supplier, GetProduct Product, double Price, int Quantity);

