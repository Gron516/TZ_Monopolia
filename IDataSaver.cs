namespace TZ_Monopolia;

public interface IDataSaver
{
    public List<Wallet> LoadData(string filePath);
    public void SaveData(string filePath, List<Wallet> wallets);

}