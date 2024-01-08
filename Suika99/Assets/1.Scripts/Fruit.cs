using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

public class Fruit : MonoBehaviour
{
    private static uint instanceID;
    private uint id;

    public ObjectPool<GameObject> pool;

    [SerializeField] private FruitType type;

    private Rigidbody2D rigid2d;
    private CircleCollider2D circle;

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        id = instanceID++;
    }

    private void OnDisable()
    {
        rigid2d.constraints = RigidbodyConstraints2D.FreezeAll;
        circle.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.TryGetComponent(out Fruit fruit) && fruit.type == type && fruit.id > id)
        {
            if (type == FruitType.Watermelon)
            {

            }
            else
            {
                GameObject highFruit = PoolsManager.Instance.fruitPools[(int)type + 1].pool.Get();
                highFruit.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                highFruit.GetComponent<CircleCollider2D>().enabled = true;

                highFruit.transform.position = Vector3.Lerp(obj.transform.position, transform.position, 0.5f);
            }

            try { fruit.pool.Release(obj); } catch { }
            try { pool.Release(gameObject); } catch { }
        }
    }

    private enum FruitType
    {
        Cherry,
        Strawberry,
        Grape,
        Shiranui,
        Persimmon,
        Apple,
        Pear,
        Peach,
        Pineapple,
        Melon,
        Watermelon,
    }
}
