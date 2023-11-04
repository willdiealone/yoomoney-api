namespace yoomoney_api.core.dtos.operation_history;

public class OperationsModel
{
    public string? operation_id;
    public string? status;
    public DateTime? datetime;
    public string? title;
    public string? pattern_id;
    public string? direction;
    public decimal? amount;
    public string? label;
    public string? type;
    public string? amount_currency;
    public string? is_sbp_operation;
}