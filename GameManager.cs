using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    // For Singleton purposes, reference to this script instance
    public static GameManager Instance;

    [Tooltip("The prefab for reference to our player instance")]
    public GameObject[] playerPrefab;

    // The local playerCar's script reference
    [HideInInspector]
    public PlayerCar localPlayer;

    public AdManager admanager;

    public GameUIManager gameUIManager;

    public DeathUI deathUi;

    public List<Transform> spawnPoints = new List<Transform>();

    private string selectedTrack = String.Empty;

    private Player[] players;
    private List<PlayerCar> myList = new List<PlayerCar>();

    public bool shouldShowAd;

    private int dontWatchAdForSessions = 0;

    //variables to show
    private int score;
    private int playerKills = 0;
    private int playerDeaths = 0;

    //variables to save
    public int money;
    public int highScore;

    private bool scoreTripled = false;
    private int gameMode;

    private byte initalPlayerNumber;
    public int scorePerOutlivedPlayer = 100;

    private int activePlayerCount;

    private int selectedCarIndex;


    // the amount what the car "heals" when jumping trough a HealthGate
    [SerializeField]
    private float HealthRepairAmount = 25f;
    [SerializeField]
    private float ZoneDamageAmount = 25f;

    [Tooltip("If the palyer's ping higher than the given value, the player will be disconnected from the server.")]
    [SerializeField]
    private int maxAcceptableLatency;

    private void Awake()
    {
        shouldShowAd = Convert.ToBoolean(PlayerPrefsManager.GetShowAd());
    }

    [Obsolete]
    void Start()
    {
        // Singleton:
        Instance = this;

        if (playerPrefab == null)
        {
            Debug.Log("PlayerPrefab in GameManager script is missing, please attach it!");
        }
        else
        {
            if (PlayerCar.LocalPlayerInstance == null)
            {
                Debug.Log("GameManager: Instanting" + playerPrefab[PlayerPrefs.GetInt("SelectedCar")].name);

                // Setting up which often we would like to sync
                PhotonNetwork.SendRate = 15;
                PhotonNetwork.SerializationRate = 15;

                PhotonNetwork.Instantiate(this.playerPrefab[PlayerPrefs.GetInt("SelectedCar")].name, GetRandomSpawnPoint(), Quaternion.identity);
            }
        }

        selectedTrack = SceneManager.GetActiveScene().name;

        gameMode = PlayerPrefsManager.GetGameMode();

        if (gameMode == 0)
        {
            StartCoroutine("UpdateActiveCarAmount");
        }
        StartCoroutine("CheckLatency");
    }

    private IEnumerator UpdateActiveCarAmount()
    {
        bool calledOnce = false;
        while (!calledOnce)
        {
            yield return new WaitForSeconds(1f);
            int numberOfActiveCars = GameObject.FindGameObjectsWithTag("Car").Length;

            if (numberOfActiveCars == 1)
            {
                CompetetiveGameOver();
                calledOnce = true;
            }
        }
    }

    private void CompetetiveGameOver()
    {
        // we are the winner :
        if (GameObject.FindGameObjectWithTag("Car").GetPhotonView().IsMine)
        {
            AddScore(100);
            deathUi.ShowWin();
            localPlayer.forwardSpeed = 0f;
            Debug.Log(localPlayer);
            RefreshScore();
        }
    }

    private IEnumerator CheckLastStandPlayer()
    {
        if (true)
        {
            yield return new WaitForSeconds(1f);
            if (PhotonNetwork.PlayerListOthers.Length == 0)
            {
                // Show : You are a winner
                //deathUi.EnableUI();
            }
        }
    }
    public static GameManager GetInstance()
    {
        return Instance;
    }

    public override void OnLeftRoom()
    {
        if (gameMode == 0)
        {

        }
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        SavePlayerData();
        PhotonNetwork.LeaveRoom();
    }

    void LoadDerby()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", selectedTrack);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(selectedTrack);
        }
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
        }
    }

    public void ShowDeathUI(string killedBy)
    {
        byte gameMode = (byte)GetGameMode();
        if (this.gameMode == 0)
        {
            deathUi.SetDeathText(killedBy);
        }
        else
        {
            deathUi.SetDeathText(killedBy);
        }
    }

    public void Rejoin()
    {
        SavePlayerData();

        if (GetGameMode() == 0)
        {
            LeaveRoom();
        }

        ResetScores();

        dontWatchAdForSessions++;

        if (dontWatchAdForSessions >= 3)
        {
            DontWatchAdForSessions(dontWatchAdForSessions);
        }
        else
        {
            // Hide deathUI
            deathUi.EnableUI(false);
            Respawn();
        }
    }

    private void ResetScores()
    {
        this.score = 0;
        this.playerKills = 0;
        this.playerDeaths = 0;
        this.scoreTripled = false;
        RefreshScore();
        gameUIManager.ResetKD();
    }

    public void Respawn()
    {
        PhotonNetwork.Instantiate(this.playerPrefab[PlayerPrefs.GetInt("SelectedCar")].name, GetRandomSpawnPoint(), Quaternion.identity);
    }


    public Vector3 GetRandomSpawnPoint()
    {
        // inorder to avoid player's spawning on top of each other
        float spawnOffset = 2f;
        float spawningHeight = 5.0f;
        int spawnID = UnityEngine.Random.Range(0, spawnPoints.Count);
        Vector3 spawnPosition = spawnPoints[spawnID].position;
        // inorder to avoid spawning on each others,when spawning the same point, little Offset
        spawnPosition.x += UnityEngine.Random.Range(0, spawnOffset);
        spawnPosition.y += spawningHeight;
        spawnPosition.z += UnityEngine.Random.Range(0, spawnOffset);

        return spawnPosition;
    }

    public float GetHealthRepair()
    {
        return HealthRepairAmount;
    }

    public float GetZoneDamage()
    {
        return -ZoneDamageAmount;
    }

    public void IncreaseKillAmount()
    {
        Debug.Log("this incraease runs on " + localPlayer.GetThisPohotonView());
        playerKills++;
        gameUIManager.ShowDestroyed(playerKills);
    }

    public void IncreaseDeathAmount()
    {
        playerDeaths++;
        gameUIManager.ShowWasted(playerDeaths);
    }
    public void GetPlayerList()
    {
        Debug.Log("this my pv is: " + localPlayer.GetThisPohotonView());
        players = PhotonNetwork.PlayerList;
        string playerss = players.ToStringFull();
        Debug.Log("this player list: " + playerss);
    }

    public void AddToMylist(PlayerCar car)
    {
        myList.Add(car);
    }
    public void GetmyListValues()
    {
        for (int i = 0; i < myList.Count; i++)
        {
            Debug.Log("thiss: " + myList[i].GetThisPohotonView());
        }
    }

    public void DontWatchAdForSessions(int skippedAmount)
    {
        if (3 <= skippedAmount)
        {
            dontWatchAdForSessions = 0;
            ShowNonRewardingAd();
        }
    }

    public void ShowNonRewardingAd()
    {
        if (shouldShowAd)
        {
            admanager.ShowAd();
        }
    }

    public void ShowRewardingAd()
    {
        if (shouldShowAd)
        {
            admanager.ShowRewardingAd();
        }
    }

    public void TripleScore()
    {
        if (!scoreTripled)
        {
            this.score *= 3;
            money = score;
            scoreTripled = true;
            Debug.Log("Triples score!");
        }
        RefreshScore();
    }

    public void AddScore(int scoreToAdd)
    {
        Debug.Log("Add score runs with: " + scoreToAdd);
        this.score += scoreToAdd;
        RefreshScore();
    }

    private void RefreshScore()
    {
        Debug.Log("Showscore : " + this.score);
        gameUIManager.ShowScore(this.score);
    }

    public void SavePlayerData()
    {
        playerData data = SaveManager.LoadData();
        int totalMoney = data.money;
        money = score;
        Debug.Log("Save money: " + money + " " + totalMoney);
        money += totalMoney;
        Debug.Log("Save money: " + money);

        if (score > highScore)
        {
            highScore = score;
        }
        SaveManager.SaveDataFromGame(this);
    }

    public void LoadPlayerData()
    {
        playerData data = SaveManager.LoadData();
        if (data != null)
        {
            highScore = data.highScore;
        }
        else
        {
            highScore = 0;
        }
    }

    public int GetScore()
    {
        return this.score;
    }

    public int GetGameMode()
    {
        return this.gameMode;
    }

    private void GameIsOver()
    {

    }

    private IEnumerator CheckLatency()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (maxAcceptableLatency < PhotonNetwork.GetPing())
            {
                Debug.Log("Ping is too high");
                LeaveRoom();
            }
        }
    }

    public bool GetShowADState()
    {
        return shouldShowAd;
    }

}
