using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    private readonly string version = "MintChoco";

    [Header("Name Fields")]
    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject createButton;

    [Header("Rooms List")]
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private RectTransform content;
    private readonly Dictionary<string, GameObject> rooms = new();

    private void Awake()
    {
        Instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.SerializationRate = 2;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CreateRoom()
    {
        SetUserName();

        string roomName = string.IsNullOrWhiteSpace(roomNameField.text) ? $"Basket_{Random.Range(1, 100)}" : roomNameField.text;
        RoomOptions room = new() { MaxPlayers = 9, IsOpen = true, IsVisible = true };
        PhotonNetwork.JoinOrCreateRoom(roomName, room, TypedLobby.Default);
    }

    public void SetUserName()
    {
        string nickName = string.IsNullOrWhiteSpace(nickNameField.text) ? $"Suika_{Random.Range(1, 100)}" : nickNameField.text;
        PhotonNetwork.NickName = nickName;
    }

    #region Photon
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        joinButton.SetActive(true);
        createButton.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(2);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public override void OnJoinedRoom()
    {

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (rooms.TryGetValue(room.Name, out GameObject temp))
                {
                    Destroy(temp);
                    rooms.Remove(room.Name);
                }
            }
            else
            {
                if (!rooms.ContainsKey(room.Name))
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, content);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = room;

                    rooms.Add(room.Name, roomPrefab);
                }
                else
                {
                    if (rooms.TryGetValue(room.Name, out GameObject temp))
                    {
                        temp.GetComponent<RoomData>().RoomInfo = room;
                    }
                }
            }
        }

        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rooms.Count * 110 + 10);
    }
    #endregion
}
