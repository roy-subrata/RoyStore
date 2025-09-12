using Api.Entities.Experiment;

namespace Api.Entities;

public class FeatureValue : BaseEntity
{
    public string Value { get; set; } = default!; // e.g. "Mahindra", "16 inch"

    public string FeatureId { get; set; }
    public Feature Feature { get; set; } = default!;

    public ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>();
}