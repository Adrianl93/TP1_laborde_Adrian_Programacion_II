using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    public static CoinSpawner Instance
    {
        get;
        private set;
    }

    [Header("Coin Prefabs")]
    [SerializeField] private GameObject bronzeCoinPrefab;
    [SerializeField] private GameObject silverCoinPrefab;
    [SerializeField] private GameObject goldCoinPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int initialActiveSpawners = 4;

    [Header("Respawn Settings")]
    [SerializeField] private float minRespawnTime = 3f;
    [SerializeField] private float maxRespawnTime = 8f;
    [SerializeField] private float spawnRadius = 2f;

    private NetworkObject[] activeCoins;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        activeCoins =
            new NetworkObject[spawnPoints.Length];

        SpawnInitialCoins();
    }

    private void SpawnInitialCoins()
    {
        int spawnCount =
            Mathf.Min(
                initialActiveSpawners,
                spawnPoints.Length);

        List<int> availableIndexes =
            new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableIndexes.Add(i);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            int randomListIndex =
                Random.Range(
                    0,
                    availableIndexes.Count);

            int spawnIndex =
                availableIndexes[randomListIndex];

            availableIndexes.RemoveAt(
                randomListIndex);

            SpawnCoin(spawnIndex);
        }
    }

    private void SpawnCoin(int spawnIndex)
    {
        Transform spawnPoint =
            spawnPoints[spawnIndex];

        GameObject prefab =
            GetRandomCoinPrefab();

        Vector2 randomOffset =
        Random.insideUnitCircle *
        spawnRadius;

        Vector3 spawnPosition =
            spawnPoint.position +
            new Vector3(
                randomOffset.x,
                0f,
                randomOffset.y);

        GameObject coin =
            Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity);

        CollectibleItem collectible =
            coin.GetComponent<CollectibleItem>();

        collectible.SetSpawnPointIndex(
            spawnIndex);

        NetworkObject networkObject =
            coin.GetComponent<NetworkObject>();

        networkObject.Spawn();

        activeCoins[spawnIndex] =
            networkObject;
    }

    private GameObject GetRandomCoinPrefab()
    {
        int roll =
            Random.Range(0, 100);

        if (roll < 60)
            return bronzeCoinPrefab;

        if (roll < 90)
            return silverCoinPrefab;

        return goldCoinPrefab;
    }

    public void NotifyCoinCollected(
        int spawnIndex)
    {
        if (!IsServer)
            return;

        activeCoins[spawnIndex] =
            null;

        StartCoroutine(
            RespawnCoinRoutine(
                spawnIndex));
    }

    private IEnumerator RespawnCoinRoutine(
        int spawnIndex)
    {
        float waitTime =
            Random.Range(
                minRespawnTime,
                maxRespawnTime);

        Debug.Log(
            $"Respawn de moneda en {waitTime:F1} segundos");

        yield return new WaitForSeconds(
            waitTime);

        SpawnCoin(spawnIndex);
    }
}