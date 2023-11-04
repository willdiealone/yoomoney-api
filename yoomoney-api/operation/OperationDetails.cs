using Newtonsoft.Json;
using yoomoney_api.core.dtos.operation_details;
using yoomoney_api.exeption;
using static System.Console;

namespace yoomoney_api.operation;

public class OperationDetails
{
    /// <summary>
    /// Идентификатор операции.
    /// Значение параметра соответствует либо значению
    /// параметра operation_id ответа метода operation-history либо,
    /// в случае если запрашивается история счета плательщика,
    /// значению поля payment_id ответа метода process-payment.
    /// </summary>
    public string? OperationId { get; }
    
    /// <summary>
    /// Статус платежа (перевода).
    /// Значение параметра соответствует значению
    /// поля status ответа метода operation-history.
    /// </summary>
    public string? Status { get; }
    
    /// <summary>
    /// Направление движения средств. Может принимать значения:
    /// in (приход);
    /// out (расход).
    /// </summary>
    public string? Direction { get; }
   
    /// <summary>
    /// Идентификатор шаблона платежа,
    /// по которому совершен платеж.
    /// Присутствует только для платежей
    /// </summary>
    public string? PatternId { get; }
    
    /// <summary>
    /// Сумма операции (сумма списания со счета).
    /// </summary>
    public decimal? Amount { get; }
    
    /// <summary>
    /// Сумма к получению.
    /// Присутствует для исходящих переводов другим пользователям.
    /// </summary>
    public decimal? AmountDue { get; }
    
    /// <summary>
    /// Сумма комиссии.
    /// Присутствует для исходящих переводов другим пользователям.
    /// </summary>
    public decimal? Fee { get; }
    
    /// <summary>
    /// Дата и время совершения операции.
    /// </summary>
    public DateTime? DateTime { get; }
    
    /// <summary>
    /// Краткое описание операции (название магазина или источник пополнения).
    /// </summary>
    public string? Title { get; }
    
    /// <summary>
    /// Номер счета отправителя перевода.
    /// Присутствует для входящих переводов от других пользователей.
    /// </summary>
    public string? Sender { get; }
    
    /// <summary>
    /// Идентификатор получателя перевода.
    /// Присутствует для исходящих переводов другим пользователям.
    /// </summary>
    public string? Recipient { get; }
    
    /// <summary>
    /// Тип идентификатора получателя перевода. Возможные значения:
    /// account — номер счета получателя в сервисе ЮMoney;
    /// phone — номер привязанного мобильного телефона получателя;
    /// email — электронная почта получателя перевода.
    /// Присутствует для исходящих переводов другим пользователям.
    /// </summary>
    public string? RecipientType { get; }
    
    /// <summary>
    /// Сообщение получателю перевода.
    /// Присутствует для переводов другим пользователям.
    /// </summary>
    public string? Message { get; }
    
    /// <summary>
    /// Комментарий к переводу или пополнению.
    /// Присутствует в истории отправителя перевода или получателя пополнения.
    /// </summary>
    public string? Comment { get; }
    
    /// <summary>
    /// Признак того, что перевод защищен кодом протекции.
    /// В ЮMoney больше нельзя делать переводы с кодом протекции,
    /// поэтому параметр всегда имеет значение false.
    /// </summary>
    public bool? Codepro { get; }
    
    /// <summary>
    /// Кодо протекции
    /// </summary>
    public string? ProtectionCode { get; }
    
    public DateTime? Expires { get; }
    public DateTime? AnswerDatetime { get; }
    
    /// <summary>
    /// Метка платежа.
    /// Присутствует для входящих и исходящих переводов другим пользователям ЮMoney,
    /// у которых был указан параметр label вызова request-payment.
    /// </summary>
    public string? Label { get; }
    
    /// <summary>
    /// Детальное описание платежа.
    /// Строка произвольного формата,
    /// может содержать любые символы и переводы строк. Необязательный параметр
    /// </summary>
    public string? Details { get; }
    
    /// <summary>
    /// Тип операции.
    /// Описание возможных типов операций см. в описании метода operation-history
    /// </summary>
    public string? Type { get; }
    
    /// <summary>
    /// Данные о цифровом товаре или бонусе
    /// </summary>
    public DigitalGood? DigitalGoods { get; }

    public OperationDetails(string token, string baseUri,string operationId)
    {
        try
        {
            
            var method = "operation-details";
            var uri = new Uri($"{baseUri}{method}");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var data = RequestData(httpClient, uri,operationId)?.Result;
            if (data is not null)
            {
                if (!string.IsNullOrEmpty(data.error) && data.error == "illegal_param_operation_id")
                    throw new AppException(message: "IllegalParamOperationId");
                
                if (!string.IsNullOrEmpty(data.error) && data.error != "illegal_param_operation_id")
                    throw new AppException(message: "TechnicalError");
                
                OperationId = data.operation_id;
                Status = data.status;
                Direction = data.direction;
                PatternId = data.pattern_id;
                Amount = data.amount;
                AmountDue = data.amount_due;
                Fee = data.fee;
                DateTime = data.datetime;
                Direction = data.direction;
                Title = data.title;
                Sender = data.sender;
                Recipient = data.recipient;
                RecipientType = data.recipient_type;
                Message = data.message;
                Comment = data.comment;
                Codepro = data.codepro;
                ProtectionCode = data.protection_code;
                Expires = data.expires;
                AnswerDatetime = data.answer_datetime;
                Label = data.label;
                Details = data.details;
                Type = data.type;

                if (data.digital_goods is not null)
                {
                    DigitalGood digitalGoods = new DigitalGood(data.digital_goods.products, data.digital_goods.bonuses);
                    DigitalGoods = digitalGoods;

                }
            }
        }
        catch (AppException e)
        {
            WriteLine(e);
        }
    }
    
    // Приватный метод для отправки запроса и получения данных
    private async Task<OperationDetailsModel?>? RequestData(HttpClient httpClient,Uri uri,string operationId)
    {
        if (!string.IsNullOrEmpty(operationId))
        {
            // Создаем словарь для параметров запроса
            var payload = new Dictionary<string, string>
            {
                ["operation_id"] = operationId
            };
            
            var requestData = payload.Select(kv =>
                new KeyValuePair<string, string>(kv.Key, kv.Value));

            // Отправялем HTTP-запрос и возвращаем ответ в response 
            var response = await httpClient.PostAsync(uri,new FormUrlEncodedContent(requestData));
            if (response.IsSuccessStatusCode)
            {
                // Читаем содержимое ответа
                var responseContent = await response.Content.ReadAsStringAsync();
                // Проверяем ответ (не является ли он пустым)
                if (!string.IsNullOrEmpty(responseContent))
                {
                    // Десериализация JSON-ответа в словарь
                    var operationDetailsDto = JsonConvert.DeserializeObject<OperationDetailsModel>(responseContent);
                    if (operationDetailsDto is not null)
                        return operationDetailsDto;
                }
            }   
        }
        else
        {
            throw new AppException(message: "IllegalParamOperationId");
        }
        
        // Возвращаем null в случаем пустого response
        return null;
    }

    public void Print()
    {
        string? digitalBonus = null; 
        string? digitalProduct = null;

        if (DigitalGoods is not null)
        {
            if (DigitalGoods.Bonuses is not null && DigitalGoods.Bonuses.Any())
            {
                digitalBonus = "digital_goods(Bonuses) --> \n";
                foreach (var item in DigitalGoods.Bonuses)
                {
                    digitalBonus += $"\n\tsecret:{item.Secret} -- {item.Serial}";
                }
            }
            if (DigitalGoods.Products is not null && DigitalGoods.Products.Any())
            {
                digitalProduct = "digital_goods(Products) --> \n";
                foreach (var item in DigitalGoods.Products)
                {
                    digitalProduct += $"\n\tmerchant_article_id{item.MerchantArticleId} -- secret:{item.Secret} -- {item.Serial}";
                }
            }
        }
        
        WriteLine("Operation details:\n" +
                  $"\toperation_id           --> {OperationId ?? "Null"}\n" +
                  $"\tstatus                 --> {Status ?? "Null"}\n" +
                  $"\tpattern_id             --> {PatternId ?? "Null"}\n" +
                  $"\tdirection              --> {Direction ?? "Null"}\n" +
                  $"\tamount                 --> {Amount?.ToString() ?? "Null"}\n" +
                  $"\tamount_due             --> {AmountDue?.ToString() ?? "Null"}\n" +
                  $"\tfee                    --> {Fee?.ToString() ?? "Null"}\n" +
                  $"\tdatetime               --> {DateTime?.ToString() ?? "Null"}\n" +
                  $"\ttitle                  --> {Title ?? "Null"}\n" +
                  $"\tsender                 --> {Sender ?? "Null"}\n" +
                  $"\trecipient              --> {Recipient ?? "Null"}\n" +
                  $"\trecipient_type         --> {RecipientType ?? "Null"}\n" +
                  $"\tmessage                --> {Message ?? "Null"}\n" + 
                  $"\tcomment                --> {Comment ?? "Null"}\n" +
                  $"\tcoderpo                --> {Codepro?.ToString() ?? "Null"}\n" +
                  $"\tprotection_code        --> {ProtectionCode ?? "Null"}\n" +
                  $"\texpires                --> {Expires?.ToString() ?? "Null"}\n" +
                  $"\tanswer_datetime        --> {AnswerDatetime?.ToString() ?? "Null"}\n" +
                  $"\tlabel                  --> {Label ?? "Null"}\n" +
                  $"\tdetails                --> {Details ?? "Null"}\n" +
                  $"\ttype                   --> {Type ?? "Null"}\n" +
                  $"\t{digitalBonus ?? "DigitalBonus is empty"}\n" +
                  $"\t{digitalProduct ?? "DigitalProduct is empty"}");
			
    }
}