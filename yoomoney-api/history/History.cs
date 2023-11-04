using System.Text;
using Newtonsoft.Json;
using yoomoney_api.core.dtos.operation_history;
using yoomoney_api.exeption;
using yoomoney_api.operation_details;
using static System.Console;

namespace yoomoney_api.history;

public class History
{
    /// <summary>
    /// Тип операции.
    /// </summary>
    public string? Type { get; }
    
    /// <summary>
    /// Метка платежа.
    /// Присутствует для входящих и исходящих переводов другим пользователям ЮMoney,
    /// у которых был указан параметр label вызова request-payment.
    /// </summary>
    public string? Label { get;}
    
    /// <summary>
    /// Вывести операции от момента времени (операции, равные from, или более поздние).
    /// Если параметр отсутствует, выводятся все операции.
    /// </summary>
    public DateTime? FromDate { get;}
    
    /// <summary>
    /// Вывести операции до момента времени (операции более ранние, чем till).
    /// Если параметр отсутствует, выводятся все операции.
    /// </summary>
    public DateTime? TillDate { get; }
    
    /// <summary>
    /// Если параметр присутствует, то будут отображены операции,
    /// начиная с номера start_record. Операции нумеруются с 0.
    /// </summary>
    public string? StartRecord { get;}
    
    /// <summary>
    /// Количество запрашиваемых записей истории операций.
    /// Допустимые значения: от 1 до 100, по умолчанию — 30.
    /// </summary>
    public int? Records { get;}
    
    /// <summary>
    /// Показывать подробные детали операции.
    /// По умолчанию false.
    /// Для отображения деталей операции требуется наличие права operation-details.
    /// </summary>
    public bool? Details { get;}
    
    /// <summary>
    /// Список операций.
    /// </summary>
    public IEnumerable<Operation>? Operations { get;}

    public History(string baseUri, string token, string? type = null,
        string? label = null, DateTime? fromDate = null, DateTime? tillDate = null,
        string? startRecord = null, int? records = null, bool? details = null)
    {
        try
        {
            var method = "operation-history";
            var uri = new Uri($"{baseUri}{method}");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        
            Type = type;
            Label = label;
        
            if (fromDate != null)
                FromDate = fromDate;

            if (tillDate != null)
                TillDate = tillDate;

            StartRecord = startRecord;
            Records = records;
            Details = details;
        
            var data = RequestData(uri,httpClient)?.Result;
            if (data is not null)
            {
                if (!string.IsNullOrEmpty(data.error))
                    throw new AppException(message: data.error);
                
                if (data.operations is not null && data.operations.Any())
                {
                    Operations = data.operations.Select(operation => new Operation(
                        amount: operation.amount,
                        status: operation.status,
                        datetime: operation.datetime,
                        label: operation.label,
                        operationId: operation.operation_id,
                        type: operation.type,
                        patternId: operation.pattern_id,
                        title: operation.title,
                        direction: operation.direction,
                        isSbpOperation: operation.is_sbp_operation,
                        amountCurrency: operation.amount_currency
                    ));
                }
            }
        }
        catch (Exception e)
        {
            WriteLine(e);
        }
    }
    
    private async Task<OperationHistoryModel?>? RequestData(Uri uri, HttpClient httpClient)
    {
        // Создаем словарь для параметров тела запроса
        var payload = new Dictionary<string, object>
        {
            
            ["type"] = Type ?? string.Empty,
            ["label"] = Label ?? string.Empty,
            ["start_record"] = StartRecord ?? string.Empty,
            ["records"] = Records ?? 0,
            ["details"] = Details ?? false
        };

        // Если есть начальная дата, добавляем её в параметры запроса
        if (FromDate != null)
            payload["from"] = FromDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");

        // Если есть конечная дата, добавляем её в параметры запроса
        if (TillDate != null)
            payload["till"] = TillDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
        // Сериализуем параметры для запроса
        string jsonContent = JsonConvert.SerializeObject(payload);
        // Инициализируем контент
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
        // Выполняем POST-запрос с передачей параметров и получаем ответ
        var response = await httpClient.PostAsync(uri, content);
        httpClient.Dispose();
        if (response.IsSuccessStatusCode)
        {
            // Читаем содержимое ответа
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseContent))
            {
                var operationHistory = JsonConvert.DeserializeObject<OperationHistoryModel>(responseContent);
                // Десериализуем JSON-ответ в словарь
                if (operationHistory is not null)
                    return operationHistory;
            }   
        }
        return null;
    }

    public void Print()
    {
        if (Operations is not null && Operations.Any())
        {
            WriteLine("\nList of operations:");
            foreach (var operation in Operations)
            {
                WriteLine($"\n\toperation id:         --> {operation.OperationId ?? "Null"}\n" +
                          $"\tstatus                --> {operation.Status ?? "Null"}\n" +
                          $"\tdatetime              --> {operation.Datetime?.ToString() ?? "Null"}\n" +
                          $"\ttitle                 --> {operation.Title ?? "Null"}\n" +
                          $"\tpattern id            --> {operation.PatternId ?? "Null"}\n" +
                          $"\tdirection             --> {operation.Direction ?? "Null"}\n" +
                          $"\tamount                --> {operation.Amount?.ToString() ?? "Null"}\n" +
                          $"\tlabel                 --> {operation.Label ?? "Null"}\n" +
                          $"\ttype                  --> {operation.Type ?? "Null"}\n" +
                          $"\tamount_currency       --> {operation.AmountCurrency ?? "Null"}\n" +
                          $"\tis_sbp_operations     --> {operation.IsSbpOperation ?? "Null"}");
            }
        }
        else
        {
            WriteLine("List of operations is empty!");
        }
    }
}