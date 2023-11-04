using yoomoney_api.operation;

namespace yoomoney_api.core.dtos.operation_details;

public class DigitalGoodModel
{
    public IEnumerable<DigitalBonus>? products;
    public IEnumerable<DigitalProduct>? bonuses;
}