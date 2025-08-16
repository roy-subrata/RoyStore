namespace Api.Models;

public class CategoryQuery : Query { }
public record GetCategory(string Id,string Name);
public record CreateCategory(string Name);
public record UpdateCategory(string Name);

