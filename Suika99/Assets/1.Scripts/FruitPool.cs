using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

public class FruitPool : MonoBehaviour
{
    [SerializeField] private GameObject fruit;

    public ObjectPool<GameObject> pool;

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(CreatePoolItem, TakePoolItem, ReleasePoolItem, collectionCheck: true, defaultCapacity: 255);
    }

    private GameObject CreatePoolItem()
    {
        GameObject poolItem = PhotonNetwork.Instantiate($"Fruits/{fruit.name}", new Vector3(0f, 100f, 0f), Quaternion.identity);
        poolItem.GetComponent<Fruit>().pool = pool;
        return poolItem;
    }

    private void TakePoolItem(GameObject item)
    {
        item.SetActive(true);
    }

    private void ReleasePoolItem(GameObject item)
    {
        item.GetComponent<Fruit>().PunSetActive(false);
    }
}
