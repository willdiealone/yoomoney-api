namespace yoomoney_api.notification;

public class PaymentListenerToYooMoney
{
    private NotificationHandler NotificationHandler { get; }
    private CancellationTokenSource CancellationTokenSource { get; }
    private string Label { get; }
    private DateTime DateTime { get; }
    private string NotificationSecret { get; }

    public PaymentListenerToYooMoney(string label, DateTime dateTime, string notification_secret)
    {
        CancellationTokenSource = new ();
        Label = label;
        DateTime = dateTime;
        NotificationSecret = notification_secret;
        NotificationHandler = new (Label, DateTime,  NotificationSecret,CancellationTokenSource);
    }

    public async Task<string> Listen(string address,int port)
    {
        var server = await NotificationHandler.GetPostByYooMoney(address,port);
        CancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(12));
        
        if (server.Contains("\nУспешно!"))
        {
            CancellationTokenSource.Cancel();
            return server + "\nСервер завершил работу";
        }
        return "\nВремя жизни сервера вышло...";
    }
}