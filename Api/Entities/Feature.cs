namespace Api.Entities.Experiment;

public class Feature : BaseEntity
{
    public string Name { get; set; } = default!; // e.g. "Fits", "Length"
    public bool IsActive { get; set; } = default;
    public ICollection<FeatureValue> Values { get; set; } = new List<FeatureValue>();
}