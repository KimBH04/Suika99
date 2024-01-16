#pragma warning disable IDE0051

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoolsManager : MonoBehaviour
{
    public static PoolsManager Instance { get; private set; }

    [SerializeField] private Transform dropPoint;
    [SerializeField] private Transform nextPoint;

    private Transform drop;
    private Transform next;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxPos;

    public List<FruitPool> fruitPools = new();

    private float horizontal;

    private void Awake()
    {
        Instance = this;
        GetComponentsInChildren(fruitPools);
    }

    private IEnumerator Start()
    {
        int rand1 = Random.Range(0, 3);
        int rand2 = Random.Range(0, 3);

        yield return new WaitForSeconds(1f);

        drop = fruitPools[rand1].pool.Get().transform;
        drop.position = dropPoint.position;

        next = fruitPools[rand2].pool.Get().transform;
        next.position = nextPoint.position;
    }

    private void Update()
    {
        dropPoint.localPosition = new Vector3(Mathf.Clamp(horizontal * Time.deltaTime * moveSpeed + dropPoint.localPosition.x, -maxPos, maxPos), 0f, 0f);

        if (drop != null)
        {
            drop.position = dropPoint.position;
        }
    }

    private void OnMove(InputValue value)
    {
        horizontal = value.Get<float>();
    }

    private void OnDrop()
    {
        if (drop == null)
        {
            return;
        }

        drop.GetComponent<Fruit>().PunSetActive(true);
        drop.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        drop.GetComponent<CircleCollider2D>().enabled = true;

        drop = null;
        StartCoroutine(DropChange());
    }

    private IEnumerator DropChange()
    {
        yield return new WaitForSeconds(0.5f);
        drop = next;
        drop.position = dropPoint.position;

        int rand = Random.Range(0, 3);
        next = fruitPools[rand].pool.Get().transform;
        next.position = nextPoint.position;

        dropPoint.localPosition = Vector3.zero;
    }
}
