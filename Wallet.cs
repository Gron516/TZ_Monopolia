namespace TZ_Monopolia;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Currency { get; set; } = "RUB";
    public decimal OpeningBalance { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
    private decimal _currentBalance;
    public decimal CurrentBalance => _currentBalance;
    
    public Wallet() {}

    public Wallet(string name, string currency, decimal openingBalance)
    {
        Name = name;
        Currency = currency;
        OpeningBalance = openingBalance;
        _currentBalance = OpeningBalance;
    }

    public bool CreateTransaction(Transaction transaction)
    {
        if (transaction.Type == TransactionType.Expense && transaction.Amount > _currentBalance)
        {
            Console.WriteLine("Недостаточно средств на счете для выполнения данной транзкации");
            return false;
        }
        
        Transactions.Add(transaction);
        
        if(transaction.Type == TransactionType.Income)
            _currentBalance += transaction.Amount;
        else
            _currentBalance -= transaction.Amount;
        return true;
    }

    public void RecalculateBalance()
    {
        var income = Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var expense = Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        _currentBalance = OpeningBalance + income - expense;
    }
}