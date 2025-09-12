namespace Api.Entities;

public class ProductFeature
{
    public string ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string FeatureValueId { get; set; }
    public FeatureValue FeatureValue { get; set; } = default!;
}