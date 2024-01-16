using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject[] anotherBaskets;
    [SerializeField] private TMP_Text[] playerNames;

    private Room room;

    private readonly Dictionary<Player, int> playerIndexes = new();
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
        foreach (var player in room.Players.OrderBy(x => x.Key).Select(x => x.Value))
        {
            if (player.NickName != PhotonNetwork.NickName)
            {
                playerNames[index].text = player.NickName;
                playerNames[index].gameObject.SetActive(true);

                playerIndexes.Add(player, index++);
            }
        }
    }

    public Transform GetBasket(Player player)
    {
        int index = playerIndexes[player];
        GameObject basket = anotherBaskets[index];
        return basket.transform;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int newPlayersIndex = room.PlayerCount - 2;
        playerIndexes.Add(newPlayer, newPlayersIndex);

        playerNames[newPlayersIndex].text = newPlayer.NickName;
        anotherBaskets[newPlayersIndex].SetActive(true);
        playerNames[newPlayersIndex].gameObject.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        anotherBaskets[room.PlayerCount - 1].SetActive(false);
        playerNames[room.PlayerCount - 1].gameObject.SetActive(false);

        foreach (var player in playerIndexes.Keys.ToArray())
        {
            if (playerIndexes[player] > playerIndexes[otherPlayer])
            {
                int index = --playerIndexes[player];
                playerNames[index].text = player.NickName;
            }
        }

        if (userIndex > playerIndexes[otherPlayer])
        {
            userIndex--;
        }

        playerIndexes.Remove(otherPlayer);
    }
}
