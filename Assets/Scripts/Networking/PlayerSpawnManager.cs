using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance
    {
        get;
        private set;
    }

    [SerializeField]
    private Transform[] spawnPoints;

    private Dictionary<ulong, int> playerSpawnIndexes =
        new Dictionary<ulong, int>();

    private int nextSpawnIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetSpawnPosition(
        ulong clientId)
    {
        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError(
                "No hay spawn points.");

            return Vector3.zero;
        }

        if (playerSpawnIndexes.ContainsKey(clientId))
        {
            int existingIndex =
                playerSpawnIndexes[clientId];

            return spawnPoints[existingIndex].position;
        }

        
        int assignedIndex =
            nextSpawnIndex % spawnPoints.Length;

        playerSpawnIndexes.Add(
            clientId,
            assignedIndex);

        nextSpawnIndex++;

        Debug.Log(
            $"Spawn asignado -> Client {clientId} => Spawn {assignedIndex}");

        return spawnPoints[assignedIndex].position;
    }

    public void ResetSpawns()
    {
        playerSpawnIndexes.Clear();

        nextSpawnIndex = 0;

        Debug.Log(
            "SpawnManager reseteado");
    }
}