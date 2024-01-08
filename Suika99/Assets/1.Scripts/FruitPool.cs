using UnityEngine;
using UnityEngine.Pool;

public class FruitPool : MonoBehaviour
{
    [SerializeField] private GameObject fruit;

    public ObjectPool<GameObject> pool;

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(CreatePoolItem, TakePoolItem, ReleasePoolItem, DestroyPoolItem, true);
    }

    private GameObject CreatePoolItem()
    {
        GameObject poolItem = Instantiate(fruit);
        poolItem.GetComponent<Fruit>().pool = pool;
        return poolItem;
    }

    private void TakePoolItem(GameObject item)
    {
        item.SetActive(true);
    }

    private void ReleasePoolItem(GameObject item)
    {
        item.SetActive(false);
    }

    private void DestroyPoolItem(GameObject item)
    {
        Destroy(item);
    }
}
