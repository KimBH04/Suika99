using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";

    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private TMP_InputField roomNameField;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;

        Debug.Log(PhotonNetwork.SendRate);

        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        string nickName = string.IsNullOrWhiteSpace(nickNameField.text) ? $"Suika_{Random.Range(1, 100)}" : nickNameField.text;
        PhotonNetwork.NickName = nickName;

        string roomName = string.IsNullOrWhiteSpace(roomNameField.text) ? $"Room_{Random.Range(1, 100)}" : roomNameField.text;
        RoomOptions room = new() { MaxPlayers = 10, IsOpen = true, IsVisible = true };
        PhotonNetwork.JoinOrCreateRoom(roomName, room, TypedLobby.Default);
    }

    public void JoinRoom()
    {


    }

    #region Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed Creating Room {returnCode}: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room!");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed Joining Room {returnCode}: {message}");
    }
    #endregion
}
