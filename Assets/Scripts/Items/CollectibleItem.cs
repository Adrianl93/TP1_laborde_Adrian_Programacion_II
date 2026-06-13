using Unity.Netcode;
using UnityEngine;

public class CollectibleItem : NetworkBehaviour
{
    [SerializeField] private int value = 1;

    public int Value => value;
}