using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviourPun
{
    [Tooltip("The value of the money, for example in score or in the shop")]
    private int moneyValue = 10;

    public int GetMoneyValue()
    {
        return this.moneyValue;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            other.gameObject.GetComponent<PlayerCar>().SetMoney(GetMoneyValue());
            if (photonView.IsMine)
                PhotonNetwork.Destroy(this.gameObject);
        }
    }

}
