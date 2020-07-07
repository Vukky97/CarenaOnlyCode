using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    // The point of this class is to simplify Getting and setting playerPrefs in code
    public static void InitializeSave()
    {
        if (!PlayerPrefs.HasKey("SaveInitialized"))
        {
            Debug.Log("check" + PlayerPrefs.GetInt("SaveInitialized"));
            PlayerPrefs.SetInt("SaveInitialized", 1);
            PlayerPrefs.Save();
            SaveManager.CreateSaveFile();
        }
    }

    public static int GetGameMode()
    {
        if (PlayerPrefs.HasKey("GameMode"))
        {
            // int to bool conversion
            return PlayerPrefs.GetInt("GameMode");
        }
        else
        {
            // the default game mode is FREE FOR ALL
            SetGameMode(1);
            return 1;
        }
    }

    public static void SetGameMode(int gameMode)
    {
        PlayerPrefs.SetInt("GameMode", gameMode);
    }

    public static string GetPlayerName()
    {
        if (PlayerPrefs.HasKey("playerName"))
        {
            return PlayerPrefs.GetString("playerName");
        }
        else
        {
            SetPlayerName("Player" + Random.Range(100, 200));
            return "Player" + Random.Range(100, 200);
        }
    }

    public static void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString("playerName", playerName);
    }

    public static int GetGraphicsLevel()
    {
        if (PlayerPrefs.HasKey("GraphicsLevel"))
        {
            return PlayerPrefs.GetInt("GraphicsLevel");
        }
        else
        {
            SetGraphicsLevel(2);
            return 2;
        }
    }

    public static void SetGraphicsLevel(int graphicsLevel)
    {
        PlayerPrefs.SetInt("GraphicsLevel", graphicsLevel);
    }

    public static float GetMusicVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            return PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            SetMusicVolume(0f);
            return 0f;
        }
    }

    public static void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    public static int GetShowAd()
    {
        if (PlayerPrefs.HasKey("ShowAD"))
        {
            // int to bool conversion
            return PlayerPrefs.GetInt("ShowAD");
        }
        else
        {
            SetShowAd(1);
            return 1;
        }
    }

    public static void SetShowAd(int showAD)
    {
        PlayerPrefs.SetInt("ShowAD", showAD);
    }

    public static int GetSelectedCar()
    {
        if (PlayerPrefs.HasKey("SelectedCar"))
        {
            return PlayerPrefs.GetInt("SelectedCar");
        }
        else
        {
            SetSelectdCar(0);
            return 0;
        }
    }
    public static void SetSelectdCar(int carID)
    {
        PlayerPrefs.SetInt("SelectedCar", carID);
    }

}
