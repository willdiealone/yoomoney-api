namespace yoomoney_api.quickpay;

public class Quickpay
{
    public string? Receiver { get; }
    public string? QuickpayForm { get; }
    public string? PaymentType { get; }
    public decimal? Sum { get; }
    public string? Label { get; }
    public string? Comment { get; }
    public string? Firstname { get; }
    public string? Lastname { get; }
    public string? Fathersname { get; }
    public string? Email { get; }
    public string? Phone { get; }
    public string? City { get; }
    public string? Street { get; }
    public string? Building { get; }
    public string? Suite { get; }
    public string? Flat { get; }
    public string? Zip { get; }
    public string? Sender { get; }
    
    
    
    public readonly string? LinkPayment;
    

    public Quickpay(string receiver, string quickpayForm, decimal sum,string label, string paymentType, string? sender = null, string? street = null,
        string? building = null, string? suite = null, string? flat = null, string? zip = null,
        string? firstname = null, string? lastname = null, string? fathersname = null,
        string? email = null, string? phone = null, string? city = null, string? comment = null)
    {
        Receiver = receiver;
        QuickpayForm = quickpayForm;
        PaymentType = paymentType;
        Sender = sender;
        Sum = sum;
        Street = street;
        Building = building;
        Suite = suite;
        Flat = flat;
        Zip = zip;
        Firstname = firstname;
        Lastname = lastname;
        Fathersname = fathersname;
        Email = email;
        Phone = phone;
        City = city;
        Comment = comment;
       
        Label = label;
        var data = RequestData().Result;
        LinkPayment = data;
    }
    
    private async Task<string?>? RequestData()
    {
        // Создаем словарь для параметров тела запроса
        var payload = new Dictionary<string, string>();
        payload["receiver"] = Receiver;
        if (Street is not null)
            payload["street"] = Street;
        if (Building is not null)
            payload["building"] = Building;
        if (Suite is not null)
            payload["suite"] = Suite;
        if (Flat is not null)
            payload["flat"] = Flat;
        if (Zip is not null)
            payload["zip"] = Zip;
        if (Firstname is not null)
            payload["firstname"] = Firstname;
        if (Lastname is not null)
            payload["lastname"] = Lastname;
        if (Fathersname is not null)
            payload["fathersname"] = Fathersname;
        if (Email is not null)
            payload["email"] = Email;
        if (Phone is not null)
            payload["phone"] = Phone;
        if (City is not null)
            payload["city"] = City;
        if (Sender is not null)
            payload["sender"] = Sender;
        payload["quickpay_form"] = QuickpayForm;
        payload["paymentType"] = PaymentType;
        payload["sum"] = Sum.ToString();
        payload["label"] = Label;
        
       var queryParams = string.Join("&", payload.Select(kv => $"{kv.Key}={kv.Value}")); 
       var uri = "https://yoomoney.ru/quickpay/confirm.xml?" + queryParams.Replace("_", "-");
       var content = new StringContent(string.Empty);
       var httpClient = new HttpClient();
       // Отправьте HTTP-запрос
       var response = await httpClient.PostAsync(uri,content);
       httpClient.Dispose();
       if (response.IsSuccessStatusCode)
       {
           // Читаем содержимое ответа
           var requestUri =  response.RequestMessage.RequestUri.ToString();
           if (!string.IsNullOrEmpty(requestUri))
           {
               return requestUri;
           }   
       }
        return uri;
    }
}