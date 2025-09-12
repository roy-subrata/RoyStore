using Api.Entities.Experiment;

namespace Api.Entities;


public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; }
    public string? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
