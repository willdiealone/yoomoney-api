namespace yoomoney_api.quickpay;

public class Quickpay
{
    public string? Receiver { get; }
    public string? QuickpayForm { get; }
    public string? Targets { get; }
    public string? PaymentType { get; }
    public decimal? Sum { get; }
    public string? FormComment { get; }
    public string? ShortDest { get; }
    public string? Label { get; }
    public string? Comment { get; }
    public string? SuccessUrl { get; }
    public bool? NeedFio { get; }
    public bool? NeedEmail { get; }
    public bool? NeedPhone { get; }
    public bool? NeedAddress { get; }
    public string? RedirectUri { get; }

    public Quickpay(string receiver, string quickpayForm, string targets, decimal sum, string? formComment = null,
        string? shortDest = null, string? label = null, string? comment = null, string? successUrl = null,
        bool? needFio = null, bool? needEmail = null, bool? needPhone = null, bool? needAddress = null,
        string? paymentType = null)
    {
        var httpClient = new HttpClient();
        Receiver = receiver;
        QuickpayForm = quickpayForm;
        Targets = targets;
        PaymentType = paymentType;
        Sum = sum;
        FormComment = formComment;
        ShortDest = shortDest;
        Label = label;
        Comment = comment;
        SuccessUrl = successUrl;
        NeedFio = needFio;
        NeedEmail = needEmail;
        NeedPhone = needPhone;
        NeedAddress = needAddress;

        var data = RequestData(httpClient)!.Result;
        RedirectUri = data;
    }
    
    private async Task<string?>? RequestData(HttpClient httpClient)
    {
        // Создаем словарь для параметров тела запроса
        var payload = new Dictionary<string, object>();
        
        if (Receiver is not null)
            payload["receiver"] = Receiver;
        if (QuickpayForm is not null)
            payload["quickpay_form"] = QuickpayForm;
        if (Targets is not null)
            payload["targets"] = Targets.Replace(" ","%20");
        if (PaymentType is not null)
            payload["paymentType"] = PaymentType;
        if (Sum is not null)
            payload["sum"] = Sum;
        if (Label is not null)
        {
            payload["label"] = Label;
        }
        var queryParams = string.Join("&", payload.Select(kv => $"{kv.Key}={kv.Value}"));
        var uri = "https://yoomoney.ru/quickpay/confirm?" + queryParams.Replace("_", "-");
        var content = new StringContent(string.Empty);
        // Отправьте HTTP-запрос
        var response = await httpClient.PostAsync(uri,content);
        httpClient.Dispose();
        if (response.IsSuccessStatusCode)
        {
            // Читаем содержимое ответа
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseContent))
            {
                return uri;
            }   
        }
        return null;
    }
}