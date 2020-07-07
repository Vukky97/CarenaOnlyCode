using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] menuMusic;
    public AudioClip[] gameMusic;

    // The menuscenes locates before this index
    public int menuScenesToIndex = 3;

    // false stands for menuMusics, true for Ingamelevel music
    private bool activeTypeOfMusic;
    private static bool startedMusic = false;

    void Awake()
    {
        GameObject[] musicGameObjects = GameObject.FindGameObjectsWithTag("Music");

        if (musicGameObjects.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        StartMusic();
    }
    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex < menuScenesToIndex)
        {
            // the pervious soundclip was menu music too, dont start again
            if (GetMusicType() == false)
            {
                return;
            }
            audioSource.clip = menuMusic[0];
            audioSource.Play();
            SetMusicType(false);
        }
        else
        {
            // dont start over music when the game level reloaded
            if (GetMusicType() == true)
            {
                return;
            }
            int randomIndex = Random.Range(0, gameMusic.Length - 1);
            audioSource.clip = gameMusic[randomIndex];
            audioSource.Play();
            SetMusicType(true);
        }
    }

    private void StartMusic()
    {
        if (!startedMusic)
        {
            Debug.Log("Start music called");
            audioSource.clip = menuMusic[0];
            audioSource.Play();
            SetMusicType(false);
            startedMusic = true;
        }
    }

    private void SetMusicType(bool musicType)
    {
        activeTypeOfMusic = musicType;
    }

    private bool GetMusicType()
    {
        return activeTypeOfMusic;
    }
}
