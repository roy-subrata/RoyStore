namespace Api.Models;
public class SupplierQuery : Query { }
public record CreateSupplier(string Name, string Phone,string Email, string Address);
public record UpdateSupplier(string Id, string Name, string Phone, string  Email, string Address);
public record GetSupplier(string Id,string Name,string Email,string Phone,string Address);
