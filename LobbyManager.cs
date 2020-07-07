using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int menuSceneIndex;

    [SerializeField]
    private int minimumPlayers;
    [SerializeField]
    private float maxWaitTime;
    [SerializeField]
    private float maxFullGameWaitTime;
    [SerializeField]
    private Text roomCountDownText;
    [SerializeField]
    private Text timerText;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    private int playerCount;
    private int roomSize;

    private void Start()
    {
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
    }

    private void PlayerCountUpdate()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDownText.text = playerCount + "/" + roomSize;

        if (playerCount == roomSize)
        {
            readyToStart = true;
        }
        else if (playerCount >= minimumPlayers)
        {
            readyToCountDown = true;
        }
        else
        {
            readyToStart = false;
            readyToCountDown = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTimer", RpcTarget.Others, timerToStartGame);
        }
    }

    [PunRPC]
    private void SyncTimer(float time)
    {
        timerToStartGame = time;
        notFullGameTimer = time;
        if (time < fullGameTimer)
        {
            fullGameTimer = time;
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    public void Update()
    {
        WaitingForPlayers();
    }

    private void WaitingForPlayers()
    {
        if (playerCount <= 1)
        {
            ResetTimer();
        }

        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        string tempTimer = string.Format("{0:0}", timerToStartGame);
        timerText.text = tempTimer;

        if (timerToStartGame <= 0f)
        {
            if (startingGame)
            {
                return;
            }
            StartGame();
        }
    }

    private void ResetTimer()
    {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    private void StartGame()
    {
        startingGame = true;
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;

        int randomTrack = Random.Range(0, 100);
        if (randomTrack < 50)
        {
            PhotonNetwork.LoadLevel(3);
        }
        else
        {
            PhotonNetwork.LoadLevel(4);
        }
    }

    public void QuitLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    // workaround because dont working in QuitLobby method
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }
}
