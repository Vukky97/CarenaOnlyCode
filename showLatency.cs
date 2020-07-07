using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class showLatency : MonoBehaviour
{
    public Text pingText;
    private int pingValue;
    void Start()
    {
        StartCoroutine("LatencyDisplay");
    }
    IEnumerator LatencyDisplay()
    {
        while (true)
        {
            pingValue = PhotonNetwork.GetPing();
            pingText.text = pingValue.ToString() + " ms";
            if (pingValue < 100)
            {
                pingText.color = Color.green;
            }
            else
            {
                pingText.color = Color.red;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
