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
            overTime -= Time.deltaTime;
            overTime = overTime < 0f ? 0f : overTime;
        }

        if (overTime >= 10f)
        {
            GameManager.Instance.IsEnd = true;
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
        }
    }
}
