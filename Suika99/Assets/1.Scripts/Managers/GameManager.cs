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

    [SerializeField] private GameObject pools;

    [Header("Other Player Displays")]
    [SerializeField] private GameObject[] otherBaskets;
    [SerializeField] private GameObject[] KOPanels;
    [SerializeField] private TMP_Text[] playerNames;

    [Header("UI")]
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private GameObject startButton;

    [SerializeField] private GameObject wonPlayerPanel;
    [SerializeField] private TMP_Text wonPlayerText;
    [SerializeField] private TMP_Text leaveCountText;

    private readonly Dictionary<Player, int> playerIndexes = new();     //Value : Another player's basket index
    private int userIndex;                                              //local client's index

    private readonly RaiseEventOptions options = new()
    {
        CachingOption = EventCaching.DoNotCache,
        Receivers = ReceiverGroup.All,
    };

    private static bool isStart;
    private static bool isEnd;

    private int remainPlayer;

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
            otherBaskets[i].SetActive(true);
        }
        
        //각 바구니마다 본인을 제외한 플레이어 정하기
        //Each baskets decide other players
        int index = 0;
        foreach (Player player in room.Players.Values)
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
                remainPlayer = room.PlayerCount;

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

                remainPlayer--;

                if (remainPlayer == 1)
                {
                    Debug.Log("Game Set!");
                    if (isEnd)
                    {
                        foreach (var player in playerIndexes)
                        {
                            if (!KOPanels[player.Value].activeSelf)
                            {
                                wonPlayerText.text = player.Key.NickName;
                                break;
                            }
                        }
                    }
                    else
                    {
                        wonPlayerText.text = PhotonNetwork.NickName;
                    }
                    wonPlayerText.text += " won!";
                    wonPlayerPanel.SetActive(true);

                    StartCoroutine(AutoExitRoom());
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

    private IEnumerator AutoExitRoom()
    {
        int count = 10;
        while (count > 0)
        {
            leaveCountText.text = $"Leave in {count} second{(count > 1 ? 's' : string.Empty)}";
            yield return new WaitForSeconds(1);
            count--;
        }
        Exit();
    }

    public void Exit()
    {
        StopAllCoroutines();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int newPlayersIndex = room.PlayerCount - 2;
        playerIndexes.Add(newPlayer, newPlayersIndex);

        playerNames[newPlayersIndex].text = newPlayer.NickName;
        otherBaskets[newPlayersIndex].SetActive(true);
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
            otherBaskets[room.PlayerCount - 1].SetActive(false);
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
        GameObject basket = otherBaskets[index];
        return basket.transform;
    }
}
