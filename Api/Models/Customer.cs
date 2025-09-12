namespace Api.Models;

public record GetCustomerResponse(string Id, string Name, string Phone, string Email, string Address);
public record CreateCustomerRequest(string Name, string Phone,string Email,  string Address);
public record UpdateCustomerRequest(string Name, string Phone,string Email, string Address);
