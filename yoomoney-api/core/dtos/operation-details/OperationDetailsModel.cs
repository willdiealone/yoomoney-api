namespace yoomoney_api.core.dtos.operation_details;

public class OperationDetailsModel
{
    public string? operation_id;
    public string? status;
    public string? pattern_id;
    public string? direction;
    public decimal? amount;
    public decimal? amount_due;
    public decimal? fee;
    public DateTime? datetime;
    public string? title;
    public string? sender;
    public string? recipient;
    public string? recipient_type;
    public string? message;
    public string? comment;
    public string? label;
    public string? protection_code;
    public DateTime? expires;
    public DateTime? answer_datetime;
    public bool? codepro;
    public string? details;
    public string? type;
    public DigitalGoodModel? digital_goods;
    public string? error;
}