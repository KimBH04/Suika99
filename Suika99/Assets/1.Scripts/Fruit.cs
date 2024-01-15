using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

public class Fruit : MonoBehaviour, IPunObservable
{
    private static uint instanceID;
    private uint id;

    public ObjectPool<GameObject> pool;

    [SerializeField] private FruitType type;

    private Rigidbody2D rigid2d;
    private CircleCollider2D circle;

    private PhotonView pv;
    private Vector3 recivePos;

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();

        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            Transform basket = GameObject.Find("Basket").transform;
            transform.parent = basket;
        }
        else
        {
            transform.parent = GameManager.Instance.GetBasket(pv.Owner.NickName);
            transform.localScale *= 0.25f;
        }
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

    private void Update()
    {
        if (!pv.IsMine)
        {
            transform.localPosition = recivePos;
        }
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
                Transform highFruit = PoolsManager.Instance.fruitPools[(int)type + 1].pool.Get().transform;
                highFruit.position = Vector3.Lerp(obj.transform.position, transform.position, 0.5f);

                highFruit.GetComponent<Fruit>().recivePos = highFruit.localPosition;
                highFruit.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                highFruit.GetComponent<CircleCollider2D>().enabled = true;
            }

            try { fruit.pool.Release(obj); } catch { }
            try { pool.Release(gameObject); } catch { }
        }
    }

    public void SetActive(bool value)
    {
        transform.localPosition = recivePos;
        pv.RPC(nameof(Active), RpcTarget.All, value);
    }

    [PunRPC]
    private void Active(bool value)
    {
        gameObject.SetActive(value);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (gameObject.activeSelf)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
            }
            else if (stream.IsReading)
            {
                recivePos = (Vector3)stream.ReceiveNext();
            }
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
