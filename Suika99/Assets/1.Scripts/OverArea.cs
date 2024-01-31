using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverArea : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    private readonly HashSet<GameObject> triggeredObjects = new();
    private int enterCount = 0;
    private Coroutine routine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruits"))
        {
            if (enterCount == 0)
            {
                routine = StartCoroutine(CountDown());
            }
            triggeredObjects.Add(collision.gameObject);
            enterCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruits"))
        {
            RemoveInstance(collision.gameObject);
        }
    }

    public void RemoveInstance(GameObject @object)
    {
        if (triggeredObjects.Remove(@object))
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
        }
    }

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(true);

        int count = 9;
        while (count > 0)
        {
            countdownText.text = count--.ToString();
            yield return new WaitForSeconds(1);
        }
        GameManager.IsEnd = true;
        enabled = false;
    }
}
