using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectUIManager : MonoBehaviour
{
    public Text SpeedText;
    public Text HealthText;
    public Text DamageText;
    public Text TurnText;

    public void ShowStats(string speed, string health, string damage, string turn)
    {
        SpeedText.text = "Speed  " + speed;
        HealthText.text = "Health  " + health;
        DamageText.text = "Damage  " + damage;
        TurnText.text = "Steering  " + turn;
    }

}
