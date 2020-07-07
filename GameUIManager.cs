using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Text destroyedCountText;
    public Text wastedCountText;
    public Text scoreText;

    public void ResetKD()
    {
        // Only visuals
        ShowDestroyed(0);
        ShowWasted(0);
    }

    public void ShowDestroyed(int valueToShow)
    {
        destroyedCountText.text = "DESTROYED:  " + valueToShow.ToString();
    }

    public void ShowWasted(int valueToShow)
    {
        wastedCountText.text = "WASTED:  " + valueToShow.ToString();
    }
    public void ShowScore(int scoreToShow)
    {
        scoreText.text = "SCORE : " + scoreToShow.ToString();
    }

}
