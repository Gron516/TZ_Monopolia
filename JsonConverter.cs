using Newtonsoft.Json;

namespace TZ_Monopolia;

public class JsonConverter:IDataSaver
{
    public List<Wallet> LoadData(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден. Возращен пустой список");
                return [];
            }

            var json = File.ReadAllText(filePath);
            var wallets = JsonConvert.DeserializeObject<List<Wallet>>(json);

            return wallets ?? [];
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Ошибка нет прав на чтении файла.");
            return new List<Wallet>();
        }
        catch (JsonReaderException jsonException)
        {
            Console.WriteLine($"Ошибка парсинга JSON: {jsonException.Message}");
            return new List<Wallet>();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Неизвестная ошибка при загрузке: {exception.Message}");
            return new List<Wallet>();
        }

    }


    public void SaveData(string filePath, List<Wallet> wallets)
    {
        try
        {
            var json = JsonConvert.SerializeObject(wallets);
        
            File.WriteAllText(filePath, json);
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Ошибка нет прав на чтении файла.");
        }
        catch (JsonReaderException jsonException)
        {
            Console.WriteLine($"Ошибка парсинга JSON: {jsonException.Message}");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Неизвестная ошибка при загрузке: {exception.Message}");
        }


    }
}