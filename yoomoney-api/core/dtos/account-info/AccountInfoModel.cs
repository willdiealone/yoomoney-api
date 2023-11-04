namespace yoomoney_api.core.dtos.account_info;

public class AccountInfoModel
{
    public string? account;
    public decimal? balance;
    public string? currency;
    public string? account_status;
    public string? account_type;
    public BalancedDetailsModel? balance_details;
    public IEnumerable<CardsLinkedModel>? cards_linked;
    public string? error;
}