using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public GameObject[] anotherBaskets;

    private Room room;

    public Dictionary<string, int> playerIndexes = new();
    private int userIndex;

    private bool isEnd;

    public bool IsEnd
    {
        get
        {
            return isEnd;
        }
        set
        {
            isEnd = value;
            Debug.Log("Game Set");
        }
    }

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;

        room = PhotonNetwork.CurrentRoom;

        userIndex = room.PlayerCount;
        int playerCount = userIndex - 1;
        for (int i = 0; i < playerCount; i++)
        {
            anotherBaskets[i].SetActive(true);
        }

        int index = 1;
        foreach (var item in room.Players.OrderBy(x => x.Key).Select(x => x.Value))
        {
            Debug.Log(item.ToString());
            playerIndexes.Add(item.NickName, index++);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        anotherBaskets[room.PlayerCount - 2].SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        anotherBaskets[room.PlayerCount - 1].SetActive(false);
    }
}
