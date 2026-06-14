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
                "PlayerSpawnManager no tiene SpawnPoints configurados.");

            return Vector3.zero;
        }

        int index =
            (int)(clientId %
            (ulong)spawnPoints.Length);

        return spawnPoints[index].position;
    }
}