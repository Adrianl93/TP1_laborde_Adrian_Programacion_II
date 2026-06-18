using Unity.Netcode;
using UnityEngine;

public class CollectibleItem : NetworkBehaviour
{
    [SerializeField] private int value = 1;

    private int spawnPointIndex;

    public int Value => value;

    public void SetSpawnPointIndex(int index)
    {
        spawnPointIndex = index;
    }

    public int GetSpawnPointIndex()
    {
        return spawnPointIndex;
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }


}