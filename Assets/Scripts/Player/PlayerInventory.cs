using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private GameObject carriedItemVisual;

    private NetworkVariable<bool> hasItem =
        new NetworkVariable<bool>(false);

    private NetworkVariable<int> carriedValue =
        new NetworkVariable<int>(0);

    public bool HasItem => hasItem.Value;
    public int CarriedValue => carriedValue.Value;

    private void Start()
    {
        if (carriedItemVisual != null)
        {
            carriedItemVisual.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        hasItem.OnValueChanged += OnHasItemChanged;

        OnHasItemChanged(false, hasItem.Value);
    }

    public override void OnNetworkDespawn()
    {
        hasItem.OnValueChanged -= OnHasItemChanged;
    }

    private void OnHasItemChanged(
        bool previousValue,
        bool newValue)
    {
        if (carriedItemVisual != null)
        {
            carriedItemVisual.SetActive(newValue);
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

        return true;
    }

    public int DeliverItem()
    {
        Debug.Log($"DeliverItem - IsServer: {IsServer}");
        if (!IsServer)
            return 0;

        if (!hasItem.Value)
            return 0;

        int value = carriedValue.Value;

        hasItem.Value = false;
        carriedValue.Value = 0;

        return value;
    }
}