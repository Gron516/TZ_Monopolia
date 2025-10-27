namespace TZ_Monopolia;

public class Transaction
{
    private Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = "";
    
    public Transaction() {}

    public Transaction(DateTime date, decimal amount, TransactionType type, string description)
    {
        if (amount < 0) throw new ArgumentException("Транзакция не может быть отрицательной");
        Date = date;
        Amount = amount;
        Type = type;
        Description = description;
    }
}