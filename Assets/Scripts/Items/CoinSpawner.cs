using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [Header("Coin Prefabs")]
    [SerializeField] private GameObject bronzeCoinPrefab;
    [SerializeField] private GameObject silverCoinPrefab;
    [SerializeField] private GameObject goldCoinPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        SpawnInitialCoins();
    }

    private void SpawnInitialCoins()
    {
        foreach (Transform point in spawnPoints)
        {
            SpawnCoin(point.position);
        }
    }

    private void SpawnCoin(Vector3 position)
    {
        GameObject prefab = GetRandomCoinPrefab();

        GameObject coin =
            Instantiate(
                prefab,
                position,
                Quaternion.identity);

        coin.GetComponent<NetworkObject>()
            .Spawn();
    }

    private GameObject GetRandomCoinPrefab()
    {
        int roll = Random.Range(0, 100);

        if (roll < 60)
            return bronzeCoinPrefab;

        if (roll < 90)
            return silverCoinPrefab;

        return goldCoinPrefab;
    }
}