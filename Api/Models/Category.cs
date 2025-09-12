namespace Api.Models;

public record GetCategoryResponse(string Id, string Name);
public record CreateCategoryRequest(string Name);
public record UpdateCategoryRequest(string Name);

