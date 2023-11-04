namespace yoomoney_api.operation;

public class DigitalGood
{
    public IEnumerable<DigitalProduct>? Products { get; }
    public IEnumerable<DigitalBonus>? Bonuses { get; }

    public DigitalGood(IEnumerable<DigitalBonus>? products, IEnumerable<DigitalProduct>? bonuses)
    {
        if (products is not null && products.Any())
        {
            Products.Select(product => new DigitalProduct(product.MerchantArticleId,product.Secret,product.Serial));
        }
        if (bonuses is not null && bonuses.Any())
        {
            Bonuses.Select(bonuses => new DigitalBonus(bonuses.Secret,bonuses.Serial));
        }
    }
}