using Photon.Pun;
using UnityEngine;

public class MoneySpawner : MonoBehaviour
{
    public GameObject MoneyPrefab;
    public static MoneySpawner Instance;

    // the distance from the floor/ground, best if it is, half of the prefab's height
    private float groundOffset = 0.35f;

    public void Start()
    {
        Instance = this;
    }

    public static MoneySpawner GetInstance()
    {
        return Instance;
    }

    public void SpawnMoney(Vector3 spawnPoint, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-2, 2), groundOffset, Random.Range(-2, 2));
            PhotonNetwork.Instantiate(MoneyPrefab.name, spawnPoint + randomOffset, Quaternion.identity);
        }

    }
}
