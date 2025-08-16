namespace Api.Models;
public class CustomerQuery : Query { }

public record GetCustomer(string Id,string Name, string Email,string Phone,string Address);
public record CreateCustomer(string Name, string Email,string Phone, string Address);
public record UpdateCustomer(string Id, string Name, string Email,string Phone, string Address);
