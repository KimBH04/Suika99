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
    private Vector3 recivePos = new(0f, 100f, 0f);
    private bool punActiveSelf;

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();

        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            transform.parent = GameObject.Find("Basket").transform;
        }
        else
        {
            transform.parent = GameManager.Instance.GetBasket(pv.Owner);
            transform.localScale *= 0.25f;
            punActiveSelf = true;
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

        if (pv.IsMine)
        {
            OverArea.RemoveInstance(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.TryGetComponent(out Fruit fruit) && fruit.type == type && fruit.id > id)
        {
            //같은 종류의 과일이 3개 이상 닿으면 예외 처리
            try
            {
                fruit.pool.Release(obj);
                pool.Release(gameObject);

                if (type == FruitType.Watermelon)
                {

                }
                else
                {
                    Transform highFruit = PoolsManager.Instance.fruitPools[(int)type + 1].pool.Get().transform;
                    highFruit.position = Vector3.Lerp(obj.transform.position, transform.position, 0.5f);

                    highFruit.GetComponent<Fruit>().PunSetActive(true);
                    highFruit.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    highFruit.GetComponent<CircleCollider2D>().enabled = true;
                }
            }
            catch { }
        }
    }

    public void PunSetActive(bool value)
    {
        pv.RPC(nameof(Active), RpcTarget.All, value);
    }

    [PunRPC]
    private void Active(bool value)
    {
        punActiveSelf = value;
        gameObject.SetActive(value);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (punActiveSelf)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
            }
            else if (stream.IsReading)
            {
                transform.localPosition = (Vector3)stream.ReceiveNext();
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
