using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown qualityDropdown;
    public Slider volumeSlider;
    public Text ADText;
    public Text gameModeText;

    int gameMode;
    bool IsAdsOn = true;

    private float selectedVolumeLevel;
    private int selectedQuality;

    void Start()
    {
        SetQuality(PlayerPrefsManager.GetGraphicsLevel());
        SetVolume(PlayerPrefsManager.GetMusicVolume());
        IsAdsOn = Convert.ToBoolean(PlayerPrefsManager.GetShowAd());
        RefreshMainMenuValues();
    }
    public void RefreshMainMenuValues()
    {
        gameMode = PlayerPrefsManager.GetGameMode();
        UpdateSelectedGameMode(gameMode);
    }

    public void RefreshSettingMenuValues()
    {
        SetQuality(PlayerPrefsManager.GetGraphicsLevel());
        qualityDropdown.value = PlayerPrefsManager.GetGraphicsLevel();
        SetVolume(PlayerPrefsManager.GetMusicVolume());
        volumeSlider.value = PlayerPrefsManager.GetMusicVolume();

        IsAdsOn = Convert.ToBoolean(PlayerPrefsManager.GetShowAd());
        ChangeADStateUI(IsAdsOn);
    }

    public void SetVolume(float volume)
    {
        selectedVolumeLevel = volume;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetQuality(int qualityLevel)
    {
        // if quality level is low, vsync is off to, and the default freamerate on mobiles is 30..
        // to avoid this, we set target framerate to greater values
        if (qualityLevel < 1)
        {
            Application.targetFrameRate = 60;
        }
        QualitySettings.SetQualityLevel(qualityLevel);
        selectedQuality = qualityLevel;
    }

    // Save the selected preferences, called when closing the settings panel.
    public void SaveMainMenuPrefs()
    {
        int actualStateADPreference = Convert.ToInt32(IsAdsOn);
        PlayerPrefsManager.SetShowAd(actualStateADPreference);
        PlayerPrefsManager.SetMusicVolume(selectedVolumeLevel);
        PlayerPrefsManager.SetGraphicsLevel(selectedQuality);
    }

    public void SwitchADState()
    {
        IsAdsOn = !IsAdsOn;
        ChangeADStateUI(IsAdsOn);
    }

    public void SwitchGameMode()
    {
        // switch the value between 0 and 1
        gameMode = 1 - gameMode;
        UpdateSelectedGameMode(gameMode);

        PlayerPrefsManager.SetGameMode(Convert.ToInt32(gameMode));
    }

    public void ChangeADStateUI(bool IsAdsOn)
    {
        if (IsAdsOn)
        {
            ADText.color = Color.green;
            ADText.text = "ON";
        }
        else
        {
            ADText.color = Color.red;
            ADText.text = "OFF";
        }
    }

    public void UpdateSelectedGameMode(int gameMode)
    {
        if (gameMode == 0)
        {
            gameModeText.text = "DEATH MATCH";
        }
        else if (gameMode == 1)
        {
            gameModeText.text = "FREE FOR ALL";
        }
        else
        {
            gameModeText.text = "ERROR";
        }
    }


}
