using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance;

    private readonly string version = "1.0";

    [Header("Name Fields")]
    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private TMP_InputField roomNameField;

    [Header("Rooms List")]
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private RectTransform content;
    private Dictionary<string, GameObject> rooms = new();

    private void Awake()
    {
        Instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;

        Debug.Log(PhotonNetwork.SendRate);

        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        SetUserName();

        string roomName = string.IsNullOrWhiteSpace(roomNameField.text) ? $"Bascket_{Random.Range(1, 100)}" : roomNameField.text;
        RoomOptions room = new() { MaxPlayers = 10, IsOpen = true, IsVisible = true };
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, roomList.Count * 110 + 10);

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
    }
    #endregion
}
