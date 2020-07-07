using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    public Text deathText;
    public GameObject deathUIPanel;
    public GameObject playAgainButtonGO;
    public GameObject TextHolderGO;

    public GameObject adText;
    public GameObject adStartButton;

    public GameObject PlacementScore;
    public GameObject ShowWinText;

    private bool shouldShowAD;

    private void Start()
    {
        shouldShowAD = Convert.ToBoolean(PlayerPrefsManager.GetShowAd());
    }
    public void SetDeathText(string playerName)
    {
        EnableUI(true);
        if (playerName == string.Empty)
        {
            playerName = "UNKNOWN";
        }
        deathText.text = "DESTROYED BY : " + playerName;
        Debug.Log("Killed by: " + playerName);
    }

    public void EnableUI(bool isEnabled)
    {
        playAgainButtonGO.SetActive(isEnabled);
        TextHolderGO.SetActive(isEnabled);

        if (shouldShowAD)
        {
            ShowADTextAndButton(isEnabled);
        }
    }

    public void ShowWin()
    {
        ShowWinText.SetActive(true);
        playAgainButtonGO.SetActive(true);

        if (shouldShowAD)
        {
            ShowADTextAndButton(true);
        }
    }

    // to avoid code duplicate
    public void ShowADTextAndButton(bool isEnabled)
    {
        adText.GetComponent<Text>().text = String.Format("Watch an AD For 3X YOUR SCORE : {0}", GameManager.GetInstance().GetScore() * 3);
        adText.SetActive(isEnabled);
        adStartButton.SetActive(isEnabled);
    }

}
