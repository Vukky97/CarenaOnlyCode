using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showErrorText : MonoBehaviour
{
    private static Text textUI;
    void Start()
    {
        textUI = GetComponent<Text>();
    }

    public static void showText(string textToShow)
    {
        textUI.text = textToShow;
    }
}
