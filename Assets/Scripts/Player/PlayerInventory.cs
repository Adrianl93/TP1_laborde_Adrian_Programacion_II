using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private GameObject carriedItemVisual;

    [Header("Drop Prefabs")]
    [SerializeField] private GameObject value1Prefab;

    [SerializeField] private GameObject value3Prefab;

    [SerializeField] private GameObject value5Prefab;

    [Header("Drop Settings")]
    [SerializeField] private float dropRadius = 2f;

    private NetworkVariable<bool> hasItem =
        new NetworkVariable<bool>(false);

    private NetworkVariable<int> carriedValue =
        new NetworkVariable<int>(0);

    public bool HasItem =>
        hasItem.Value;

    public int CarriedValue =>
        carriedValue.Value;

    private void Start()
    {
        if (carriedItemVisual != null)
        {
            carriedItemVisual.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        hasItem.OnValueChanged +=
            OnHasItemChanged;

        OnHasItemChanged(
            false,
            hasItem.Value);
    }

    public override void OnNetworkDespawn()
    {
        hasItem.OnValueChanged -=
            OnHasItemChanged;
    }

    private void OnHasItemChanged(
        bool previousValue,
        bool newValue)
    {
        if (carriedItemVisual != null)
        {
            carriedItemVisual.SetActive(
                newValue);
        }
    }

    public bool PickupItem(int value)
    {
        if (!IsServer)
            return false;

        if (hasItem.Value)
            return false;

        hasItem.Value = true;

        carriedValue.Value = value;

        Debug.Log(
            $"Pickup item value={value}");

        return true;
    }

    public int DeliverItem()
    {
        if (!IsServer)
            return 0;

        if (!hasItem.Value)
            return 0;

        int value =
            carriedValue.Value;

        hasItem.Value = false;

        carriedValue.Value = 0;

        return value;
    }

    public void DropItem()
    {
        if (!IsServer)
            return;

        if (!hasItem.Value)
            return;

        GameObject prefabToSpawn =
            GetPrefabByValue(
                carriedValue.Value);

        if (prefabToSpawn == null)
        {
            Debug.LogError(
                $"No existe prefab para value={carriedValue.Value}");

            return;
        }

        Vector3 randomOffset =
            new Vector3(
                Random.Range(
                    -dropRadius,
                    dropRadius),
                0.5f,
                Random.Range(
                    -dropRadius,
                    dropRadius));

        Vector3 spawnPosition =
            transform.position +
            randomOffset;

        GameObject droppedItem =
            Instantiate(
                prefabToSpawn,
                spawnPosition,
                Quaternion.identity);

        NetworkObject networkObject =
            droppedItem.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn();
        }

        Debug.Log(
            $"Item dropeado | Value={carriedValue.Value}");

        hasItem.Value = false;

        carriedValue.Value = 0;
    }

    private GameObject GetPrefabByValue(
        int value)
    {
        switch (value)
        {
            case 1:
                return value1Prefab;

            case 3:
                return value3Prefab;

            case 5:
                return value5Prefab;
        }

        return null;
    }

    public void ResetInventory()
    {
        if (!IsServer)
            return;

        hasItem.Value = false;

        carriedValue.Value = 0;
    }
}