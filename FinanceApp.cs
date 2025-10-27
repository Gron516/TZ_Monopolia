namespace TZ_Monopolia;

public class FinanceApp
{
    public IDataSaver DataSaver { get; set; }

    public FinanceApp(IDataSaver dataSaver) => 
        DataSaver = dataSaver;

    public void SaveData(string filePath, List<Wallet> wallets) => 
        DataSaver.SaveData(filePath, wallets);

    public List<Wallet> LoadData(string filePath) => 
        DataSaver.LoadData(filePath);

    public List<Wallet> GenerateData(List<Wallet> wallets)
    {
        var rnd = new Random();
        var now = DateTime.Now;

        wallets =
        [
            new Wallet { Name = "Наличка", Currency = "RUB", OpeningBalance = 300 },
            new Wallet { Name = "Карта", Currency = "RUB", OpeningBalance = 300 }
        ];

        string[] incomes = { "Зарплата", "Акции", "Продажа", "Аренда" };
        string[] expenses = { "Кварплата", "Продукты", "Транспорт", "Досуг" };

        foreach (var w in wallets)
            for (int i = 0; i < 40; i++)
            {
                var isIncome = rnd.NextDouble() < 0.3;
                var date = now.AddDays(-rnd.Next(0, 180));

                if (isIncome)
                {
                    decimal amount = rnd.Next(100, 2000);
                    w.CreateTransaction(new Transaction(date, amount, TransactionType.Income,
                        incomes[rnd.Next(0, incomes.Length)]));
                }
                else
                {
                    decimal amount = rnd.Next(5, 400);
                    w.CreateTransaction(new Transaction(date, amount, TransactionType.Expense,
                        expenses[rnd.Next(0, expenses.Length)]));
                }
            }

        return wallets;
    }

    public void ShowData(List<Wallet> wallets, int month, int year)
    {
        Console.WriteLine($"Отчет за {month:D2}.{year}");

        var allTransactions = wallets.SelectMany(w => w.Transactions.Select(t => (wallet: w, transaction: t)))
            .Where(p => p.transaction.Date.Month == month && p.transaction.Date.Year == year)
            .ToList();

        if (allTransactions.Count == 0)
        {
            Console.WriteLine("Нет транзакций за этот месяц.");
            return;
        }

        var groupedTransactions = allTransactions
            .GroupBy(p => p.transaction.Type)
            .Select(g => new
            {
                Type = g.Key,
                Total = g.Sum(p => p.transaction.Amount),
                Items = g.OrderBy(p => p.transaction.Date).ToList()
            })
            .OrderByDescending(g => g.Total);

        foreach (var g in groupedTransactions)
        {
            Console.WriteLine($"{g.Type} - (Итого {g.Total:F2})");
            foreach (var i in g.Items)
            {
                Console.WriteLine(
                    $"{i.transaction.Date:dd.MM} | {i.wallet.Name,-10} | {i.transaction.Amount,7:F2} | {i.transaction.Description}");
                Console.WriteLine();
            }
        }

        Console.WriteLine("3 самые большие траты за указанный месяц:");
        foreach (var w in wallets)
        {
            var top = w.Transactions
                .Where(t => t.Type == TransactionType.Expense && t.Date.Month == month && t.Date.Year == year)
                .OrderByDescending(t => t.Amount)
                .Take(3)
                .ToList();

            Console.WriteLine($"\n{w.Name} ({w.Currency}) - баланс {w.CurrentBalance}");
            if (top.Count == 0)
            {
                Console.WriteLine("Нет расходов за этот месяц.");
                continue;
            }

            var i = 1;
            foreach (var t in top)
            {
                Console.WriteLine($"{i++}. {t.Date:dd.MM} - {t.Amount,7:F2} | {t.Description}");
            }
        }
    }

    public void AddWallet(List<Wallet> wallets)
    {
        Console.WriteLine("Добавление нового кошелька");
        Console.Write("Введите название кошелька:");
        var inputName = Console.ReadLine();
        var name = string.IsNullOrWhiteSpace(inputName) ? "Без названия" : inputName.Trim();

        Console.WriteLine("Введите валюту (по умолчанию рубли):");
        var inputCurrency = Console.ReadLine();
        var currency = string.IsNullOrWhiteSpace(inputCurrency) ? "RUB" : inputCurrency.Trim().ToUpper();

        Console.WriteLine("Введите начальный баланс:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
            balance = 0;

        wallets.Add(new Wallet { Name = name, Currency = currency, OpeningBalance = balance });
        Console.WriteLine($"Кошелек {name} добавлен.");
    }

    public void AddTransaction(List<Wallet> wallets)
    {
        if (!wallets.Any())
        {
            Console.WriteLine("Нет ни одного кошелька");
            return;
        }

        Console.WriteLine("Добавление транзакции");
        for (int i = 0; i < wallets.Count; i++)
            Console.WriteLine($"{i + 1}. {wallets[i].Name}, {wallets[i].Currency}, баланс: {wallets[i].CurrentBalance:F2}");

        Console.WriteLine("Выберете кошелек:");
        if (!int.TryParse(Console.ReadLine(), out int walletIndex) || walletIndex < 1 || walletIndex > wallets.Count)
        {
            Console.WriteLine("Неверный выбор.");
            return;
        }

        Wallet wallet = wallets[walletIndex - 1];

        Console.WriteLine("Выберете тип транзакции (1 доход, 2 расход");
        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || (typeChoice != 1 && typeChoice != 2))
        {
            Console.WriteLine("Неверный тип");
            return;
        }

        var type = typeChoice == 1 ? TransactionType.Income : TransactionType.Expense;

        Console.WriteLine("Сумма:");
        var amount = ParseMoney(Console.ReadLine());

        Console.WriteLine("Описание:");
        var description = Console.ReadLine() ?? "";

        var date = DateTime.Now;
        wallet.CreateTransaction(new Transaction(date, amount, type, description));
    }

    private static decimal ParseMoney(string money)
    {
        money = money.Trim();
        money = money.Replace(",", ".");

        if (decimal.TryParse(money, out var result))
        {
            if (result > 0)
                return Math.Round(result, 2);

            else
                return Math.Round(-result, 2);
        }
        
        Console.WriteLine("Неверный формат денег. Будет введен 0");
        return 0;
    }
}