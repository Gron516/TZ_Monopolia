namespace TZ_Monopolia;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var wallets = new List<Wallet>();
        var app = new FinanceApp(new JsonConverter());
        Console.WriteLine("Приложение для учета финансов");

        while (true)
        {
            Console.WriteLine("==================================");
            Console.WriteLine("1. Сгенерировать случайные данные");
            Console.WriteLine("2. Загрузить данные из файла JSON");
            Console.WriteLine("3. Добавить кошелек");
            Console.WriteLine("4. Добавить транзакцию");
            Console.WriteLine("5. Показать отчет за месяц");
            Console.WriteLine("6. Показать список кошельков");
            Console.WriteLine("7. Сохранить данные в формате JSON");
            Console.WriteLine("0. Выход");
            Console.WriteLine("==================================");
            
            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                    case "1":
                        wallets = app.GenerateData(wallets);
                        break;
                    
                    case "2":
                        Console.WriteLine("Введите путь к файлу");
                        var pathSave = Console.ReadLine();
                        if (pathSave != null)
                        {
                            wallets = app.LoadData(pathSave);
                            foreach (var w in wallets)
                                w.RecalculateBalance();
                        }
                        else
                        {
                            Console.WriteLine("Неверный путь");
                        }

                        break;

                    case "3":
                        app.AddWallet(wallets);
                        break;
                    
                    case "4":
                        app.AddTransaction(wallets);
                        break;
                    
                    case "5":
                        Console.WriteLine("Введите месяц и год (например 10 2025): ");
                        var parts = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length < 2 || !int.TryParse(parts[0], out var month) ||
                            !int.TryParse(parts[1], out var year))
                        {
                            Console.WriteLine("Неверный ввод");
                            return;
                        }
                        app.ShowData(wallets,month,year);
                        break;
                    
                    case "6":
                        foreach (var w in wallets) 
                            Console.WriteLine($"{w.Name} ({w.Currency}) - баланс: {w.CurrentBalance:F2}");
                        break;
                    
                    case "7":
                        Console.WriteLine("Введите путь к файлу");
                        var pathLoad = Console.ReadLine();
                        if (pathLoad != null)
                        {
                            app.SaveData(pathLoad,wallets);
                        }
                        else
                        {
                            Console.WriteLine("Неверный путь");
                        }
                        break;
                        
                    case "0":
                        Console.WriteLine("Хороших финансов");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
            }
        }
    }
}
