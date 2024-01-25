using ExitGames.Client.Photon;
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

    private static Room room;

    [SerializeField] private GameObject[] anotherBaskets;
    [SerializeField] private GameObject[] KOPanels;
    [SerializeField] private TMP_Text[] playerNames;

    [SerializeField] private GameObject startButton;
    [SerializeField] private TMP_Text countDownText;

    [SerializeField] private GameObject pools;

    private readonly Dictionary<Player, int> playerIndexes = new();
    private int userIndex;

    private readonly RaiseEventOptions options = new()
    {
        CachingOption = EventCaching.DoNotCache,
        Receivers = ReceiverGroup.All,
    };

    private static bool isStart;
    private static bool isEnd;

    public static bool IsStart
    {
        get
        {
            return isStart;
        }
        set
        {
            room.IsVisible = value;
            isStart = value;
        }
    }

    public static bool IsEnd
    {
        get
        {
            return isEnd;
        }
        set
        {
            isEnd = value;
            PhotonNetwork.RaiseEvent(1, PhotonNetwork.LocalPlayer, Instance.options, SendOptions.SendReliable);
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

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }

        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void StartGame()
    {
        if (room.PlayerCount == 1)
        {
            //Display warning text
            Debug.LogWarning("No one came to this room!");
            return;
        }

        PhotonNetwork.RaiseEvent(0, null, options, SendOptions.SendReliable);
    }

    private void OnEvent(EventData data)
    {
        byte eventCode = data.Code;
        switch (eventCode)
        {
            case 0:
                IsStart = true;
                startButton.SetActive(false);

                StartCoroutine(CountDown());
                countDownText.gameObject.SetActive(true);
                break;

            case 1:
                Player endedPlayer = (Player)data.CustomData;
                if (endedPlayer != PhotonNetwork.LocalPlayer)
                {
                    int index = playerIndexes[endedPlayer];
                    KOPanels[index].SetActive(true);
                }
                else
                {
                    KOPanels[^1].SetActive(true);
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.DestroyPlayerObjects(endedPlayer);
                }
                break;

            default:
                if (eventCode < 200)
                {
                    Debug.LogWarning($"Recieved unknown event code! : {{{eventCode}}}");
                }
                break;
        }
    }

    private IEnumerator CountDown()
    {
        int count = 3;
        while (count > 0)
        {
            countDownText.text = count--.ToString();
            yield return new WaitForSeconds(1);
        }

        countDownText.gameObject.SetActive(false);
        pools.SetActive(true);
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
        if (isStart)
        {
            int index = playerIndexes[otherPlayer];
            KOPanels[index].SetActive(true);
        }
        else
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

            if (PhotonNetwork.IsMasterClient)
            {
                startButton.SetActive(true);
            }
        }
    }

    public Transform GetBasket(Player player)
    {
        int index = playerIndexes[player];
        GameObject basket = anotherBaskets[index];
        return basket.transform;
    }
}
