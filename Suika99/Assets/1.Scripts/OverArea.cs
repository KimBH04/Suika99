using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverArea : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    private static readonly HashSet<GameObject> triggeredObjects = new();
    private int enterCount = 0;
    private Coroutine routine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruits"))
        {
            if (enterCount == 0)
            {
                routine = StartCoroutine(CountDown());
                countdownText.gameObject.SetActive(true);
            }
            triggeredObjects.Add(collision.gameObject);
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

            if (enterCount == 0)
            {
                StopCoroutine(routine);
                countdownText.gameObject.SetActive(false);
            }
            RemoveInstance(collision.gameObject);
        }
    }

    public static void RemoveInstance(GameObject @object)
    {
        triggeredObjects.Remove(@object);
    }

    private IEnumerator CountDown()
    {
        int count = 10;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }
        GameManager.IsEnd = true;
        enabled = false;
    }
}
