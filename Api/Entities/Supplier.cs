using Api.Entities.Experiment;

namespace Api.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; }
    public string Address { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = [];
}