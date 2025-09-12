namespace Api.Models;

public record CreateSupplierRequest(string Name, string Email, string Phone, string Address);
public record UpdateSupplierRequest(string Name, string Email, string Phone, string Address);
public record GetSupplierResponse(string Id, string Name, string Email, string Phone, string Address);
