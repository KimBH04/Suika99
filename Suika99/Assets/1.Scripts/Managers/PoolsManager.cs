#pragma warning disable IDE0051

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoolsManager : MonoBehaviour
{
    public static PoolsManager Instance { get; private set; }

    [SerializeField] private Transform Basket;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private Transform nextPoint;
    [SerializeField] private Transform holdPoint;

    private Transform drop;
    private Transform next;
    private Transform hold;
    private bool isChanged;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxPos;

    public List<FruitPool> fruitPools = new();

    private float horizontal;

    private void Awake()
    {
        Instance = this;
        GetComponentsInChildren(fruitPools);
    }

    private void Start()
    {
        int rand1 = Random.Range(0, 3);
        int rand2 = Random.Range(0, 3);

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

        Rigidbody2D rigid2d = drop.GetComponent<Rigidbody2D>();
        CircleCollider2D circle = drop.GetComponent<CircleCollider2D>();

        rigid2d.constraints = RigidbodyConstraints2D.None;
        circle.enabled = true;

        drop.parent = Basket;

        drop = null;
        StartCoroutine(DropChange());
    }

    private void OnHold()
    {
        if (isChanged || drop == null)
        {
            return;
        }

        if (hold == null)
        {
            hold = drop;
            drop = next;

            int rand = Random.Range(0, 3);
            next = fruitPools[rand].pool.Get().transform;
            next.position = nextPoint.position;
        }
        else
        {
            (drop, hold) = (hold, drop);
        }

        hold.position = holdPoint.position;
        isChanged = true;
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

        isChanged = false;
    }
}
