using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPSScript : MonoBehaviour
{
    [SerializeField]
    private Text fpsText;
    [SerializeField] 
    private float UIRefreshRate = 0.5f;

    private float timer;

    private void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText.text = "FPS: " + fps;
            timer = Time.unscaledTime + UIRefreshRate;

            if (fps < 30)
            {
                fpsText.color = Color.red;
            }
            else
            {
                fpsText.color = Color.green;
            }
        }
    }
}
