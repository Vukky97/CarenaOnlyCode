using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveManager
{
    public static void CreateSaveFile()
    {
        playerData data = new playerData();
        SaveData(data);
    }

    public static void SaveDataFromMenu(MoneyManager moneyManager)
    {
        playerData data = new playerData(moneyManager);
        SaveData(data);
        for (int i = 0; i < data.unlockedCars.Count; i++)
        {
            Debug.Log(data.unlockedCars);
        }
    }
    public static void SaveDataFromGame(GameManager gameManager)
    {
        playerData data = new playerData(gameManager);
        SaveData(data);
    }

    public static void SaveData(playerData dataToSave)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string filePath = Application.persistentDataPath + "/data.save";
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, dataToSave);
        fileStream.Close();
    }

    public static playerData LoadData()
    {
        string filePath = Application.persistentDataPath + "/data.save";
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            playerData data = binaryFormatter.Deserialize(fileStream) as playerData;
            fileStream.Close();

            return data;
        }
        else
        {
            Debug.Log("Savefind does not found");
            return null;
        }
    }

}

[System.Serializable]
public class playerData
{
    public int money;
    public int highScore;
    public List<bool> unlockedCars;
    public playerData()
    {
        this.money = 0;
        this.highScore = 0;
        this.unlockedCars = new List<bool>() { true, false, false, false, false, false };
    }

    public playerData(MoneyManager MM)
    {
        this.money = MM.GetMoney();
        this.unlockedCars = new List<bool>() { true, false, false, false, false, false };
    }
    public playerData(GameManager GM)
    {
        this.money = GM.money;
        this.highScore = GM.highScore;
    }
}
