using System.Text;
using yoomoney_api.core.dtos.account_info;
using yoomoney_api.exeption;
using static System.Console;

namespace yoomoney_api.account;

public class Account
{
    /// <summary>
    /// Номер счета пользователя.
    /// </summary>
    public string? AccountNumber { get; }
    
    /// <summary>
    /// Баланс счета пользователя.
    /// </summary>
    public decimal? Balance { get; }
    
    /// <summary>
    /// Код валюты счета пользователя. (рубль РФ по стандарту ISO 4217).
    /// </summary>
    public string? Currency { get; }
    
    /// <summary>
    /// Статус пользователя. Возможные значения:
    /// anonymous — анонимный счет;
    /// named — именной счет;
    /// identified — идентифицированный счет.
    /// </summary>
    public string? AccountStatus { get; }
    
    /// <summary>
    /// Тип счета пользователя. Возможные значения:
    /// personal — счет пользователя в ЮMoney;
    /// professional — профессиональный счет в ЮMoney.
    /// </summary>
    public string? AccountType { get; }
    
    /// <summary>
    /// По умолчанию этот блок отсутствует.
    /// Блок появляется, если сейчас или когда-либо ранее были зачисления в очереди,
    /// задолженности, блокировки средств.
    /// </summary>
    public BalanceDetails? BalanceDetails { get; }
    
    /// <summary>
    /// Информация о привязанных банковских картах.
    /// Если к счету не привязано ни одной карты, параметр отсутствует.
    /// Если к счету привязана хотя бы одна карта,
    /// параметр содержит список данных о привязанных картах.
    /// </summary>
    public IEnumerable<Card>? CardsLinked { get; }

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="baseUri"></param>
    /// <param name="token"></param>
    /// <param name="method"></param>
    public Account(string baseUri, string token)
    {
        try
        {
            var method = "account-info";
            var uri = new Uri($"{baseUri}{method}");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var data = RequestData(httpClient,uri)?.Result;
            if (data is not null)
            {
                if (!string.IsNullOrEmpty(data.error))
                    throw new AppException(message: data.error);
            
                AccountNumber = data.account;
                Balance = data.balance;
                Currency = data.currency;
                AccountStatus = data.account_status;
                AccountType = data.account_type;
            
                if (data.balance_details is not null)
                    BalanceDetails = new BalanceDetails
                    {
                        Available = data.balance_details.available,
                        Blocked = data.balance_details.blocked,
                        Debt = data.balance_details.debt,
                        DepositionPending = data.balance_details.deposition_pending,
                        Total = data.balance_details.total,
                        Hold = data.balance_details.hold
                    };

                if (data.cards_linked is not null && data.cards_linked.Any())
                {
                    CardsLinked.Select(card => new Card(
                        panFragment: card.PanFragment,
                        type: card.Type));
                }
            }
            else
                throw new AppException(message: "InvalidToken");
        }
        catch (AppException e)
        {
            WriteLine(e);

        }
    }  

    // Приватный метод для отправки запроса и получения данных
    private async Task<AccountInfoModel?>? RequestData(HttpClient httpClient,Uri uri)
    {
        // Создаем HttpContent с пустым содержимым
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");

        // Отправялем HTTP-запрос и возвращаем ответ в response 
        var response = await httpClient.PostAsync(uri,content);
        httpClient.Dispose();
        if (response.IsSuccessStatusCode)
        {
            // Читаем содержимое ответа
            var responseContent = await response.Content.ReadAsStringAsync();
            // Проверяем ответ (не является ли он пустым)
            if (!string.IsNullOrEmpty(responseContent))
            {
                // Десериализация JSON-ответа в словарь
                var accountInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountInfoModel>(responseContent);
                if (accountInfo is not null)
                    return accountInfo;
            }   
        }
        return null;
    }

    public void Print()
    {
        string? balanceDetails = null;
        string? linkedCards = null;
        if (BalanceDetails is not null)
        {
            balanceDetails = new string($"\t--> total:                   {BalanceDetails.Total?.ToString() ?? "Null"}\n" +
                                            $"\t--> available:               {BalanceDetails.Available?.ToString() ?? "Null"}\n" +
                                            $"\t--> deposition_pending:      {BalanceDetails.DepositionPending?.ToString() ?? "Null"}\n" +
                                            $"\t--> blocked:                 {BalanceDetails.Blocked?.ToString() ?? "Null"}\n" +
                                            $"\t--> debt:                    {BalanceDetails.Debt?.ToString() ?? "Null"}\n" +
                                            $"\t--> hold:                    {BalanceDetails.Hold?.ToString() ?? "Null"}");
        }

        if (CardsLinked is not null && CardsLinked.Any())
        {
            foreach (var card in CardsLinked)
            {
                linkedCards += $"\n\tPart of the card number: {card.PanFragment} -- Type card: {card.Type}";
            }
        }
        
        WriteLine($"Account number:                             {AccountNumber}\n" + 
                          $"Account balance:                            {Balance}\n" + 
                          $"Account currency code in ISO 4217 format:   {Currency}\n" + 
                          $"Account status:                             {AccountStatus}\n" + 
                          $"Account type:                               {AccountType}\n" +
                          $"Extended balance information:\n" +
                          $"{balanceDetails ?? "balanceDetails is empty"}\n" +
                          $"Information about linked bank cards:\n" +
                          $"{linkedCards ?? "\tNo card is linked to the account"}");
    }
}