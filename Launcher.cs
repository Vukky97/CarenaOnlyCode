using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Tooltip("The maximum capacity of the rooms")]
    [SerializeField]
    private byte roomPlayerCount = 6;
    [Tooltip("The UI Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    private Text connectionInfoText;

    [SerializeField]
    private int LobbySceneIndex;
    [SerializeField]
    private int ShopSceneIndex;

    // only check save exists once per session
    private static bool firstRun = true;

    private bool isConnecting;

    // Version number to separate players from each other
    private string gameVersion = "2.3";

    void Awake()
    {
        // automaticly change scene when the masterclients chages, forexample in competetive Last Man Standing mode looby -> game track
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        connectionInfoText = progressLabel.GetComponent<Text>();

        if (firstRun)
        {
            InitializeSave();
            firstRun = false;
        }

    }

    // Inital save creation in order to avoid can't save player data
    private void InitializeSave()
    {
        PlayerPrefsManager.InitializeSave();
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            connectionInfoText.color = Color.green;
            connectionInfoText.text = "Connected to (" + PhotonNetwork.CloudRegion + ")";
        }
        else if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.color = Color.green;
            connectionInfoText.text = "Connecting...";
        }
        else
        {
            connectionInfoText.color = Color.red;
            connectionInfoText.text = "Can't connect to server";
        }
    }
    public int GetSelectedGameMode()
    {
        int actualGameMode = PlayerPrefsManager.GetGameMode();
        return actualGameMode;
    }

    public void Connect()
    {
        isConnecting = true;

        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        Debug.Log("Connect: " + GetSelectedGameMode());

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("PhotonNetwork Is Connected");
            JoinToGameRooms();
        }
        else
        {
            Debug.Log("PhotonNetwork Is not Connected");
            // connect to the photon master/name server.
            PhotonNetwork.GameVersion = this.gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Debug.Log("Connecting with mode: " + GetSelectedGameMode());

            JoinToGameRooms();
            isConnecting = false;
        }
    }

    private void JoinToGameRooms()
    {
        PhotonHashtable expectedCustomRoomProperties = new PhotonHashtable() { { "mode", GetSelectedGameMode() } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, roomPlayerCount);

    }

    // we dont find or cant connect to any room, so we create one.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = roomPlayerCount;
        roomOptions.CustomRoomProperties = new PhotonHashtable() { { "mode", GetSelectedGameMode() } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "mode" };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    //Called whehn we created a room and joins to it, and when another joins, too
    public override void OnJoinedRoom()
    {
        if (GetSelectedGameMode() == 0)
        {
            PhotonNetwork.LoadLevel(LobbySceneIndex);
        }
        else
        {
            int randomTrack = UnityEngine.Random.Range(0, 100);

            if (randomTrack < 50)
            {
                PhotonNetwork.LoadLevel(3);
            }
            else
            {
                PhotonNetwork.LoadLevel(4);
            }
        }
    }

    public void OpenShopScene()
    {
        SceneManager.LoadScene(ShopSceneIndex);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        isConnecting = false;
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

}
