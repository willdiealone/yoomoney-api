namespace yoomoney_api.operation;

public class DigitalProduct
{
    /// <summary>
    /// Идентификатор товара в системе продавца. Присутствует только для товаров.
    /// </summary>
    public string MerchantArticleId { get; }
    
    /// <summary>
    /// Серийный номер товара (открытая часть пин-кода, кода активации или логин).
    /// </summary>
    public string Serial { get; }
    
    /// <summary>
    /// Секрет цифрового товара (закрытая часть пин-кода, кода активации, пароль или ссылка на скачивание).
    /// </summary>
    public string Secret { get; }

    public DigitalProduct(string merchantArticleId, string serial, string secret)
    {
        MerchantArticleId = merchantArticleId;
        Serial = serial;
        Secret = secret;
    }
}