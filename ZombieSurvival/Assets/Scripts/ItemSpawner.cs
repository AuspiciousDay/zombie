using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] ItemPrefabs;
    private List<GameObject>[] itemPools;

    public float interval = 3f;
    private float timer = 0f;

    private void Awake()
    {
        itemPools = new List<GameObject>[ItemPrefabs.Length];
        for (int i = 0; i < itemPools.Length; i++)
        {
            itemPools[i] = new List<GameObject>();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            timer = 0f;
            int index = Random.Range(0, ItemPrefabs.Length);
            if (itemPools[index].Count == 0)
            {
                GameObject newItemGo = Instantiate(ItemPrefabs[index]);
                itemPools[index].Add(newItemGo);

                newItemGo.SetActive(false);
                IItem newItem = newItemGo.GetComponent<IItem>();

                if (newItem != null)
                {
                    newItem.OnUse.AddListener(() => itemPools[index].Add(newItemGo));
                }
            }
            Vector3 position = Vector3.zero;
            position.x = Random.Range(-10f, 10f);
            position.z = Random.Range(-10f, 10f);
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                position = hit.position;
                position.y = 0.5f;

                GameObject newGo = itemPools[index][0];
                newGo.transform.position = position;
                newGo.SetActive(true);
                itemPools[index].RemoveAt(0);
            }
            else
            {
                Debug.Log("N");
            }
        }
    }
}

/*
public class ItemSpawner : MonoBehaviour
{
    public int itemCount = 5;
    public GameObject[] ItemPrefabs;
    public GameObject[] Items;

    private void Start()
    {
        Items = new GameObject[itemCount];
        CreateItem();
    }

    private void CreateItem()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                hit.position += new Vector3(0f, 1f, 0f);
                Items[i] = Instantiate(ItemPrefabs[Random.Range(0, ItemPrefabs.Length)]);
                Items[i].SetActive(false);
                Items[i].transform.position = hit.position;
                Items[i].SetActive(true);
            }
        }
    }
}
*/