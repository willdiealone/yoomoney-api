using yoomoney_api.exeption;
using static System.Console;
namespace yoomoney_api.authorize;

public class Authorize
{
    public string TokenUrl { get; }
    // Метод авторизации
    public Authorize(string clientId,string redirectUri,string[] scope)
    {
        var httpClient = new HttpClient();
        // Формируем URL-адрес для запроса авторизации
        var authorizeUrl = $"https://yoomoney.ru/oauth/authorize?client_id={clientId}&response_type=code" +
                           $"&redirect_uri={redirectUri}&scope={string.Join(" ", scope).Replace(" ","%20")}";
        // Выводим сообщение для пользователя с URL-адресом авторизации
        WriteLine("\nVisit this website and confirm the application authorization request:");
        WriteLine($"{authorizeUrl}");
        Write("\nEnter the full address of the page after the redirect=\n=> ");
        // Считываем ввод пользователя
        var code = ReadLine();
        // Проверяем, что введенный код не является пустым и содержит "code="
        if (!string.IsNullOrEmpty(code))
        {
            // Извлекаем код авторизации из строки и удаляем пробелы
            code = code.Substring(code.IndexOf("code=", StringComparison.Ordinal) + 5).Replace(" ", "");
        }
        else
        {
            throw new AppException(message: "EmptyRedirectUrlCode");
        }
        
        // Формируем URL-адрес для запроса токена
        var tokenUrl = $"https://yoomoney.ru/oauth/token?code={code}&client_id={clientId}" +
                       $"&grant_type=authorization_code&redirect_uri={redirectUri}";
        
        // метод получения токена
        var accessToken =  GetAccessToken(tokenUrl,httpClient).Result;
        
        // Проверяем, является ли токен доступа (accessToken) пустым или отсутствующим
        if (string.IsNullOrEmpty(accessToken))
            throw new AppException(message: "EmptyRedirectUrlCode");
        
        // Если токен доступа не пустой, выводим сообщение с токеном
        WriteLine("\nYour access token:");
        WriteLine($"{accessToken}");
        TokenUrl = accessToken;
    }

    private async Task<string?> GetAccessToken(string tokenUrl,HttpClient httpClient)
    {
        // Выполняем асинхронный POST-запрос к указанному URL (tokenUrl) без тела запроса
        var response = await httpClient.PostAsync(tokenUrl, null);
        httpClient.Dispose();
        // Проверяем, что ответ от сервера имеет успешный статус (HTTP 200 OK)
        if (response.IsSuccessStatusCode)
        {
            // Считываем содержимое ответа в виде строки
            var content = await response.Content.ReadAsStringAsync();
            // Пытаемся десериализовать JSON-строку в словарь (Dictionary) с ключами и значениями строк
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            // Проверяем, что json не является пустым и содержит ключ "access_token"
            if (json is not null && json.TryGetValue("access_token", out var accessToken))
            {
                // Если ключ "access_token" найден, возвращаем значение токена доступа
                return accessToken;
            }
        }
        // Если что-то пошло не так, возвращаем null
        return null;
    }
}