using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }


}
