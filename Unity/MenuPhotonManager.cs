using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuPhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _joinToRoomButton;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _pingText;

    public void ConnectToMasterButton()
    {
        //if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
        //{
            PhotonNetwork.ConnectUsingSettings();    
        //}
        _nameText.text = "Ваш ник: " + PlayerPrefs.GetString("Nickname");
    }
    public void CreateRoomButton()
    {
        RoomOptions roomSettings = new RoomOptions();
        roomSettings.MaxPlayers = 20;
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    public override void OnConnectedToMaster()
    {
        print("Connected to master!");
        PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
        _pingText.text = "Ваш пинг: " + PhotonNetwork.GetPing().ToString() + " ms";
        _joinToRoomButton.interactable = true;
    }
    public override void OnJoinedRoom()
    {
        print("Joined to room!");
        PhotonNetwork.LoadLevel("Game");
    }
}
