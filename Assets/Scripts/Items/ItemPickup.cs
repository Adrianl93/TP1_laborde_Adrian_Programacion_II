using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class ItemPickup : NetworkBehaviour
{
    private PlayerInventory inventory;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner)
            return;

        CollectibleItem item =
            other.GetComponent<CollectibleItem>();

        if (item == null)
            return;

        if (inventory.HasItem)
            return;

        RequestPickupServerRpc(
            item.NetworkObjectId);
    }

    [ServerRpc]
    private void RequestPickupServerRpc(
        ulong itemNetworkObjectId)
    {
        if (!NetworkManager.Singleton.SpawnManager
            .SpawnedObjects.TryGetValue(
                itemNetworkObjectId,
                out NetworkObject networkObject))
        {
            return;
        }

        CollectibleItem item =
            networkObject.GetComponent<CollectibleItem>();

        if (item == null)
            return;

        if (!inventory.PickupItem(item.Value))
            return;

        // Avisar al CoinSpawner qué SpawnPoint quedó libre
        CoinSpawner.Instance.NotifyCoinCollected(
            item.GetSpawnPointIndex());

        // Eliminar moneda de la red
        networkObject.Despawn(true);

        Debug.Log(
            $"Jugador {OwnerClientId} recogió un objeto");
    }
}