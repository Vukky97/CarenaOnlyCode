using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    private int maxNameLength = 15;
    private string errorMessage = string.Empty;

    void Start()
    {
        string defaultName = string.Empty;
        InputField inputField = this.GetComponent<InputField>();
        if (inputField != null)
        {
            defaultName = PlayerPrefsManager.GetPlayerName();

            // In the interest of avoid modify values trough PlayerPerfs :
            if (maxNameLength < defaultName.Length)
            {
                errorMessage = string.Format("Player Name is too long. Maximum allowed : {0} character", maxNameLength);
                showErrorText.showText(errorMessage);
                // Only keep the first allowedNumber char
                defaultName = defaultName.Substring(0, 15);
            }
            inputField.text = defaultName;
        }
        PhotonNetwork.NickName = defaultName;
    }

    // Set the player name and save it the PlayerPerfs way
    public void SetPlayerName(string chosenName)
    {
        if (string.IsNullOrEmpty(chosenName))
        {
            Debug.LogError("Player Name is null or empty");
            errorMessage = "Player Name is null or empty";
            showErrorText.showText(errorMessage);
            return;
        }
        if (maxNameLength < chosenName.Length)
        {
            Debug.Log("Player Name is longer than allowed");
            
            errorMessage = string.Format("Player Name is too long. Maximum allowed : {0} character", maxNameLength);
            showErrorText.showText(errorMessage);
            return;
        }
        PhotonNetwork.NickName = chosenName;
        PlayerPrefsManager.SetPlayerName(chosenName);
    }

}
