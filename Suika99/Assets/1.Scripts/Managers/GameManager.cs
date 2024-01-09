using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject[] anotherBaskets;
    [SerializeField] private TMP_Text[] playerNames;

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
        }
    }

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;

        room = PhotonNetwork.CurrentRoom;

        userIndex = room.PlayerCount - 1;
        int playerCount = userIndex;
        for (int i = 0; i < playerCount; i++)
        {
            anotherBaskets[i].SetActive(true);
        }

        int index = 0;
        foreach (var nickName in room.Players.OrderBy(x => x.Key).Select(x => x.Value.NickName))
        {
            if (nickName != PhotonNetwork.NickName)
            {
                playerNames[index].text = nickName;
                playerNames[index].gameObject.SetActive(true);

                playerIndexes.Add(nickName, index++);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int newPlayersIndex = room.PlayerCount - 2;
        playerIndexes.Add(newPlayer.NickName, newPlayersIndex);

        playerNames[newPlayersIndex].text = newPlayer.NickName;
        anotherBaskets[newPlayersIndex].SetActive(true);
        playerNames[newPlayersIndex].gameObject.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        anotherBaskets[room.PlayerCount - 1].SetActive(false);
        playerNames[room.PlayerCount - 1].gameObject.SetActive(false);

        foreach (var nickName in playerIndexes.Keys.ToArray())
        {
            if (playerIndexes[nickName] > playerIndexes[otherPlayer.NickName])
            {
                int index = --playerIndexes[nickName];
                playerNames[index].text = nickName;
            }
            Debug.Log($"{nickName} {playerIndexes[nickName]}");
        }

        if (userIndex > playerIndexes[otherPlayer.NickName])
        {
            userIndex--;
        }

        playerIndexes.Remove(otherPlayer.NickName);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
