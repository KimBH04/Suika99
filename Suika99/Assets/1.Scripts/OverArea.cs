using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverArea : MonoBehaviour
{
    private int enterCount = 0;
    private float overTime = 0f;

    private void Update()
    {
        if (enterCount > 0)
        {
            overTime += Time.deltaTime;
        }
        else
        {
            overTime = 0f;
        }

        if (overTime >= 10f)
        {
            GameManager.Instance.IsEnd = true;
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruits"))
        {
            enterCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruits"))
        {
            enterCount--;
            if (enterCount < 0)
            {
                enterCount = 0;
            }
        }
    }
}
