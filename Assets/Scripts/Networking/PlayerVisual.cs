using Unity.Netcode;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material hostMaterial;
    [SerializeField] private Material clientMaterial;

    public override void OnNetworkSpawn()
    {
        if (OwnerClientId == 0)
        {
            meshRenderer.material = hostMaterial;
        }
        else
        {
            meshRenderer.material = clientMaterial;
        }
    }
}