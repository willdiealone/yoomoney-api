using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static System.Console;

namespace yoomoney_api.notification;

public class NotificationHandler
{
	/// <summary>
	/// Для переводов из кошелька — p2p-incoming
	/// Для переводов с произвольной карты — card-incoming.
	/// </summary>
	private string NotificationType { get; set; }
	
	/// <summary>
	/// Идентификатор операции в истории счета получателя.
	/// </summary>
	private string OperationId { get; set; }
	
	/// <summary>
	/// Сумма операции.
	/// </summary>
	private string Amount { get; set; }
	
	/// <summary>
	/// 	Сумма, которая списана со счета отправителя.
	/// </summary>
	private string WithdrawAmount { get; set; }
	
	/// <summary>
	/// Код валюты счета пользователя. Всегда 643 (рубль РФ согласно ISO 4217).
	/// </summary>
	private string Currency { get; set; }
	
	/// <summary>
	/// Дата и время совершения перевода.
	/// </summary>
	private string DateTime { get; set; }
	
	/// <summary>
	/// Для переводов из кошелька — номер счета отправителя.
	/// Для переводов с произвольной карты — параметр содержит пустую строку.
	/// </summary>
	private string Sender { get; set; }
	
	/// <summary>
	/// Признак того, что перевод защищен кодом протекции.
	/// В ЮMoney больше нельзя делать переводы с кодом протекции, поэтому параметр всегда имеет значение false.
	/// </summary>
	private string Codepro { get; set; }
	
	/// <summary>
	/// Метка платежа. Если метки у платежа нет, параметр содержит пустую строку.
	/// </summary>
	private string Label { get; set; }
	
	/// <summary>
	/// Флаг означает, что пользователь не получил перевод.
	/// На счете получателя достигнут лимит доступного остатка, поэтому перевод заморожен.
	/// Сумма замороженных средств отображается в поле hold ответа метода account-info.
	/// </summary>
	private string Unaccepted { get; set; }
	
	/// <summary>
	/// Имя отправителя перевода. Если не запрашивались, параметры содержат пустую строку.
	/// </summary>
	private string Firstname { get; set; }
	
	/// <summary>
	/// Фамилия отправителя перевода. Если не запрашивались, параметры содержат пустую строку.
	/// </summary>
	private string Lastname { get; set; }
	
	/// <summary>
	/// Отчество отправителя перевода. Если не запрашивались, параметры содержат пустую строку.
	/// </summary>
	private string Fathersname { get; set; }
	
	/// <summary>
	/// Адрес электронной почты отправителя перевода.
	/// Если email не запрашивался, параметр содержит пустую строку.
	/// </summary>
	private string Email { get; set; }
	
	/// <summary>
	/// Телефон отправителя перевода.
	/// Если телефон не запрашивался, параметр содержит пустую строку.
	/// </summary>
	private string Phone { get; set; }
	
	/// <summary>
	/// Адрес, указанный отправителем перевода для доставки.
	/// Если адрес не запрашивался, параметры содержат пустую строку.
	/// </summary>
	private string City { get; set; }
	
	/// <summary>
	/// Адрес,(улица) указанная отправителем перевода для доставки.
	/// Если адрес (улица) не запрашивался, параметры содержат пустую строку.
	/// </summary>
	private string Street { get; set; }
	
	/// <summary>
	/// Адрес,(строение) указанная отправителем перевода для доставки.
	/// Если адрес (строение) не запрашивался, параметры содержат пустую строку.
	/// </summary>
	private string Building { get; set; }
	
	/// <summary>
	/// Адрес,(номер помещения) указанная отправителем перевода для доставки.
	/// Если адрес (номер помещения) не запрашивался, параметры содержат пустую строку.
	/// </summary>
	private string Suite { get; set; }
	
	/// <summary>
	/// Адрес,(квартира) указанная отправителем перевода для доставки.
	/// Если адрес (квартира) не запрашивался, параметры содержат пустую строку.
	/// </summary>
	private string Flat { get; set; }
	
	/// <summary>
	/// Почтовый индекс отправителя.
	/// Если Почтовый индекс не запрашивался, параметры содержат пустую строку.
	/// </summary>
	private string Zip { get; set; }

	private string LabelToCheck { get; set; }
	private DateTime DataTimelToCheck { get; set; }
	private string NotificationSecret { get; }
	private bool firstClientAccepted { get; set; }
	private CancellationTokenSource Token { get; }
	
	public NotificationHandler(string label, DateTime dateTime, string notification_secret,CancellationTokenSource token)
	{
		bool firstClientAccepted = false;
		LabelToCheck = label;
		DataTimelToCheck = dateTime;
		NotificationSecret = notification_secret;
		Token = token;
	}

	public async Task<string> GetPostByYooMoney(string address, int port)
	{
		TcpListener listener;
		try
		{
			IPAddress ip;
			if (IPAddress.TryParse(address, out ip))
			{
				WriteLine("\nIP-appec: " + ip);
			}
			else
			{
				IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(address);
				ip = ipHostInfo.AddressList[0];
				WriteLine("\nДоменное имя: " + address + ", IP: " + ip);
			}
			IPEndPoint ipEndPoint = new IPEndPoint(ip, port);  
			// Создаем TcpListener для прослушивания
			listener = new TcpListener(ipEndPoint);
			// Начинаем прослушивание
			listener.Start();
			WriteLine("Сервер запущен. Ожидание подключений...");
			
			// Если клиент первый и токен не отменен
			while (!Token.IsCancellationRequested &&  !firstClientAccepted)
			{
				Token.Token.ThrowIfCancellationRequested();
				// Ожидаем подключения клиента
				var client = await listener.AcceptTcpClientAsync(Token.Token);
				if (client.Connected)
				{
					WriteLine("\nПодключен клиент.");
					// Запускаем обработку клиента в фоновом режиме
					var result = await HandleClient(client);
					if (result.Contains("\nУспешно!"))
					{
						listener.Stop();
						return result;
					}
					if (result.Contains("Не удалось распознать POST-запрос..."))
						return result;
					if (result.Contains("Не текущий платеж..."))
						WriteLine(result);
				}
			}
		}
		// Обрабатываем возможные исключения
		catch (Exception ex)
		{
			if (ex.GetType() != typeof(OperationCanceledException))
			{
				WriteLine($"Ошибка при подключении клиента: {ex.Message}");
			}
		}
		return null;
	}

	async Task<string> HandleClient(TcpClient tcpClient)
	{
		try
		{
			// Используем блок using для автоматического закрытия ресурсов (NetworkStream и StreamReader)
			using (NetworkStream networkStream = tcpClient.GetStream())
			using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
			{
				// Асинхронно читаем HTTP-запрос из клиента
				var httpRequest = await ReadHttpPostRequestAsync(reader);
				// Если HTTP-запрос не пустой, выводим его в консоль

				if (httpRequest is null)
				{
					return"Не удалось распознать POST-запрос...";	
				}
				else if (httpRequest is not null && httpRequest.Value.Item2)
				{
					firstClientAccepted = httpRequest.Value.Item2;
					return httpRequest.Value.Item1 + "\nУспешно!";
				}
				else if (httpRequest is not null && !httpRequest.Value.Item2)
				{
					firstClientAccepted = httpRequest.Value.Item2;
					return "Не текущий платеж...";
				}
			}
		}
		// Обрабатываем возможные исключения
		catch (Exception ex)
		{
			WriteLine($"Ошибка при обработке запроса: {ex.Message}");
		}
		// В блоке finally закрываем соединение с клиентом независимо от успешности обработки
		finally
		{
			tcpClient.Close();
			tcpClient.Dispose();
		}

		return null;
	}

	async Task<(string?,bool)?> ReadHttpPostRequestAsync(StreamReader reader)
	{
		// Проверяем, что reader не является null
		if (reader is not null)
		{	
			// Создаем StringBuilder для построения строки запроса
			StringBuilder requestBuilder = new StringBuilder();
		
			// Создаем буфер для чтения данных
			char[] buffer = new char[4096];
			int bytesRead;
		
			// Читаем данные из потока до тех пор, пока не достигнем конца HTTP-заголовка
			while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				// Добавляем прочитанные данные в StringBuilder
				requestBuilder.Append(buffer, 0, bytesRead);
			
				// Если достигнут конец HTTP-заголовка, прекращаем чтение
				if (requestBuilder.ToString().Contains("\r\n\r\n"))
				{
					break;  // Прекращаем чтение, когда достигнут конец HTTP-заголовка
				}
			}
		
			// Проверяем, что запрос начинается с "POST"
			if (requestBuilder.Length > 4 && requestBuilder[0] == 'P' && requestBuilder[3] == 'T') // если ответ это POST запрос =>
			{
				var request = requestBuilder.ToString();
				var parameters = HttpUtility.ParseQueryString(request);
				NotificationType = request.Substring(request.IndexOf("notification_type=") + "notification_type=".Length).Split("&")[0];
				OperationId = parameters["operation_id"];
				WithdrawAmount = parameters["withdraw_amount"];
				Firstname = parameters["firstname"];
				Lastname = parameters["lastname"];
				Fathersname = parameters["fathersname"];
				Email = parameters["email"];
				Unaccepted = parameters["unaccepted"];
				Phone = parameters["phone"];
				City = parameters["city"];
				Street = parameters["street"];
				Building = parameters["building"];
				Suite = parameters["suite"];
				Flat = parameters["flat"];
				Zip = parameters["zip"];
				Sender = parameters["sender"];
				Codepro = parameters["codepro"];
				Amount =  parameters["amount"];
				Currency = parameters["currency"];
				Label = parameters["label"];
				DateTime = parameters["datetime"];
				DateTime datetimeToConsole = System.DateTime.ParseExact(DateTime, "yyyy-MM-ddTHH:mm:ssZ",
					CultureInfo.InvariantCulture); 
				string? sha1_hashFromRequest = parameters["sha1_hash"];
				
				var res = "HTTP Requests\n-------------\n\nPOST /" +
				          "\nТекущий платеж:\n" +
				          $"\tNotificationType   --> {(NotificationType != "" ? NotificationType : " Null")}\n" +
				          $"\tOperationId        --> {(OperationId != "" ? OperationId : " Null")}\n" +
				          $"\tDateTime           --> {datetimeToConsole:yyyy-MM-dd HH:mm}\n" +
				          $"\tAmount             --> {(Amount != "" ? Amount : " Null")}\n" + 
				          $"\tWithdrawAmount     --> {(WithdrawAmount != "" ? WithdrawAmount : " Null")}\n" +
				          $"\tFirstname          --> {(Firstname != "" ? Firstname : " Null")}\n" +
				          $"\tLastname           --> {(Lastname != "" ? Lastname : " Null")}\n" +
				          $"\tFathersname        --> {(Fathersname != "" ? Fathersname : " Null")}\n" +
				          $"\tEmail              --> {(Email != "" ? Email : " Null")}\n" +
				          $"\tPhone              --> {(Phone != "" ? Phone : " Null")}\n" +
				          $"\tCity               --> {(City != "" ? City : " Null")}\n" +
				          $"\tStreet             --> {(Street != "" ? Street : " Null")}\n" +
				          $"\tBuilding           --> {(Building != "" ? Building : " Null")}\n" +
				          $"\tSuite              --> {(Suite != "" ? Suite : " Null")}\n" +
				          $"\tFlat               --> {(Flat != "" ? Flat : " Null")}\n" +
				          $"\tZip                --> {(Zip != "" ? Zip : " Null")}\n" +
				          $"\tSender             --> {(Sender != "" ? Sender : " Null")}\n" +
				          $"\tUnaccepted         --> {(Unaccepted != "" ? Unaccepted : " Null")}\n" +
				          $"\tCodepro            --> {(Codepro != "" ? Codepro : " Null")}\n" +
				          $"\tCurrency           --> {(Currency != "" ? Currency : " Null")}\n" +
				          $"\tLabel              --> {(Label  != "" ? Label : " Null")}\n";

				if (!string.IsNullOrEmpty(NotificationType))
				{
					var paramString = string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}&{7}&{8}",
						NotificationType,OperationId,Amount,Currency,DateTime,Sender,Codepro,NotificationSecret,Label);
					var paramStringHash1 = GetHash(paramString);
					if (paramStringHash1 == sha1_hashFromRequest && Label == LabelToCheck && datetimeToConsole.Date == DataTimelToCheck.Date)
						return new(res, true);	
				}
				// Возвращаем кортеж значений
				return new(res, false);
			}
		}
		// Возвращаем null, если не удалось распознать POST-запрос
		return null;
	}

	string GetHash(string args) 
	{
		var sha = new SHA1CryptoServiceProvider();
		byte[] data = sha.ComputeHash(Encoding.Default.GetBytes(args));
             
		var sBuilder = new StringBuilder();
 
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		return sBuilder.ToString(); 
	}	
}