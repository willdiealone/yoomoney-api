namespace yoomoney_api.account;

public class BalanceDetails
{
    /// <summary>
    /// Общий баланс
    /// </summary>
    public double? Total { get; set; }
    
    /// <summary>
    /// Доступный баланс
    /// </summary>
    public double? Available { get; set; }
    /// <summary>
    /// Ожидающий внесения баланс
    /// </summary>
    public double? DepositionPending { get; set; }
    
    /// <summary>
    /// Заблокированный (недоступный) баланс
    /// </summary>
    public double? Blocked { get; set; }
    
    /// <summary>
    /// Долг или задолженность
    /// </summary>
    public double? Debt { get; set; }
    
    /// <summary>
    /// Замороженный баланс
    /// </summary>
    public double? Hold { get; set; }
    
    public BalanceDetails(
        double? total = null,
        double? available = null,
        double? depositionPending = null,
        double? blocked = null,
        double? debt = null,
        double? hold = null)
    {
        Total = total;
        Available = available;
        DepositionPending = depositionPending;
        Blocked = blocked;
        Debt = debt;
        Hold = hold;
    }
}