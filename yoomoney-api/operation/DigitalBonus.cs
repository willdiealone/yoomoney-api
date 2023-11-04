namespace yoomoney_api.operation;

public class DigitalBonus
{
    /// <summary>
    /// Серийный номер товара (открытая часть пин-кода, кода активации или логин).
    /// </summary>
    public string Serial { get; }
    
    /// <summary>
    /// Секрет цифрового товара (закрытая часть пин-кода, кода активации, пароль или ссылка на скачивание).
    /// </summary>
    public string Secret { get; }

    public DigitalBonus(string serial, string secret)
    {
        Serial = serial;
        Secret = secret;
    }
}