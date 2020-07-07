using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public Text moneyText;
    private int moneyToShow = 0;
    private List<bool> lockState;
    private void Start()
    {
        RefreshData();
        GetLockState();
        SetLockstate(1);
    }
    public int GetMoney()
    {
        return moneyToShow;
    }
    public void SetMoney(int moneyToSet)
    {
        this.moneyToShow = moneyToSet;
    }

    public List<bool> GetLockState()
    {
        return lockState;
    }

    //load the lockstates based on save file
    public void SetLockstate(int indexToUnlock)
    {
        playerData data = SaveManager.LoadData();
        if (data != null)
        {
            lockState = data.unlockedCars;
            SaveData();
        }
        Debug.Log("save data is null");

    }
    public void RefreshData()
    {
        playerData data = SaveManager.LoadData();
        if (data != null)
        {
            moneyToShow = data.money;
            moneyText.text = moneyToShow.ToString();
            lockState = data.unlockedCars;
        }
        else
        {
            moneyText.text = moneyToShow.ToString();
            lockState = new List<bool>() { true, false, false, false, false, false };
        }
    }

    public void SaveData()
    {
        SaveManager.SaveDataFromMenu(this);
        Debug.Log(this.lockState);
    }

    public void ChangeMoneyAmount(int change)
    {
        if (GetMoney() - change < 0)
        {
            Debug.Log("Purchase failed, not anoguh money");
        }
        else
        {
            SetMoney(GetMoney() - change);
            Debug.Log("Purchase sucess");
        }
    }

}
