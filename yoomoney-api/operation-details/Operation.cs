namespace yoomoney_api.operation_details;

public class Operation
{
    /// <summary>
    /// Идентификатор операции.
    /// </summary>
    public string? OperationId { get; set; }
    
    /// <summary>
    /// Статус платежа (перевода). Может принимать следующие значения:
    /// success — платеж завершен успешно;
    /// refused — платеж отвергнут получателем или отменен отправителем;
    /// in_progress — платеж не завершен или перевод не принят получателем.
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Дата и время совершения операции.
    /// </summary>
    public DateTime? Datetime { get; set; }
    
    /// <summary>
    /// Краткое описание операции (название магазина или источник пополнения).
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Идентификатор шаблона, по которому совершен платеж.
    /// Присутствует только для платежей.
    /// </summary>
    public string? PatternId { get; set; }
    
    /// <summary>
    /// Направление движения средств. Может принимать значения:
    /// in (приход);
    /// out (расход).
    /// </summary>
    public string? Direction { get; set; }
    
    /// <summary>
    /// Сумма операции.
    /// </summary>
    public decimal? Amount { get; set; }
    
    /// <summary>
    /// Метка платежа.
    /// Присутствует для входящих и исходящих переводов другим пользователям ЮMoney,
    /// у которых был указан параметр label вызова request-payment.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// Тип операции. Возможные значения:
    /// payment-shop — исходящий платеж в магазин;
    /// outgoing-transfer — исходящий P2P-перевод любого типа;
    /// deposition — зачисление;
    /// incoming-transfer — входящий перевод.
    /// </summary>
    public string? Type { get; set; }
    
    /// <summary>
    /// Валюта.
    /// </summary>
    public string? AmountCurrency { get; }
    
    /// <summary>
    /// Операция СБП
    /// </summary>
    public string? IsSbpOperation { get; }

    public Operation(string? amountCurrency, string? isSbpOperation, string? operationId = null, string? status = null, DateTime? datetime = null,
        string? title = null, string? patternId = null, string? direction = null, decimal? amount = null,
        string? label = null, string? type = null)
    {
        AmountCurrency = amountCurrency;
        IsSbpOperation = isSbpOperation;
        OperationId = operationId;
        Status = status;
        Datetime = datetime;
        Title = title;
        PatternId = patternId;
        Direction = direction;
        Amount = amount;
        Label = label;
        Type = type;
    }
}