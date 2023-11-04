using yoomoney_api.history;
using yoomoney_api.operation;

namespace yoomoney_api.account;

public class Client
{
    /// <summary>
    /// Базовый адрес
    /// </summary>
    private readonly string _baseUri;
    
    /// <summary>
    /// Токен доступа
    /// </summary>
    private readonly string _token;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="token"></param>
    /// <param name="baseUri"></param>
    public Client(string token, string baseUri = "https://yoomoney.ru/api/")
    {
        _token = token;
        _baseUri = baseUri;
    }
    
    /// <summary>
    /// Получение информации о состоянии счета пользователя.
    /// </summary>
    /// <returns></returns>
    public Account GetAccountInfo()
    {
        return new Account(_baseUri, _token);
    }

    /// <summary>
    /// Метод позволяет просматривать историю операций (полностью или частично) в постраничном режиме.
    /// Записи истории выдаются в обратном хронологическом порядке: от последних к более ранним
    /// </summary>
    /// <param name="type"></param>
    /// <param name="label"></param>
    /// <param name="fromDate"></param>
    /// <param name="tillDate"></param>
    /// <param name="startRecord"></param>
    /// <param name="records"></param>
    /// <param name="details"></param>
    /// <returns></returns>
    public History GetOperationHistory(
        string? type = null,
        string? label = null,
        DateTime? fromDate = null,
        DateTime? tillDate = null,
        string? startRecord = null,
        int? records = null,
        bool details = false)
    {
        return new History(_baseUri,
            _token, type, label, fromDate, tillDate, startRecord, records, details);
    }
    
    /// <summary>
    /// Позволяет получить детальную информацию об операции из истории.
    /// </summary>
    /// <param name="operationId"></param>
    /// <returns></returns>
    public OperationDetails GetOperationDetails(string operationId)
    {
        return new OperationDetails( _token, _baseUri, operationId);
    }
}