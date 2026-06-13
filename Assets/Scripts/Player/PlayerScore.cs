using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    private NetworkVariable<int> score =
        new NetworkVariable<int>(0);

    public int Score => score.Value;

    public void AddPoints(int amount)
    {
        if (!IsServer)
            return;

        score.Value += amount;

        Debug.Log(
            $"Jugador {OwnerClientId} tiene {score.Value} puntos");
    }
}