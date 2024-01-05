using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    private RoomInfo roomInfo;
    private TMP_Text roomInfoText;

    public RoomInfo RoomInfo
    {
        get
        {
            return roomInfo;
        }
        set
        {
            roomInfo = value;
            roomInfoText.text =  $"{roomInfo.Name} ({roomInfo.PlayerCount} / {roomInfo.MaxPlayers})";
            GetComponent<Button>().onClick.AddListener(() => OnEnterRoom(roomInfo.Name));
        }
    }

    private void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnterRoom(string roomName)
    {
        PhotonManager.Instance.SetUserName();

        RoomOptions room = new() { MaxPlayers = 10, IsOpen = true, IsVisible = true };
        PhotonNetwork.JoinOrCreateRoom(roomName, room, TypedLobby.Default);
    }
}
