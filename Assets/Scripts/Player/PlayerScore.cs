using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    private NetworkVariable<int> score =
        new NetworkVariable<int>(0);

    private NetworkVariable<int> playerNumber =
        new NetworkVariable<int>(0);

    public int Score =>
        score.Value;

    public int PlayerNumber =>
        playerNumber.Value;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        AssignPlayerNumber();
    }

    private void AssignPlayerNumber()
    {
        if (OwnerClientId == 0)
        {
            playerNumber.Value = 1;
        }
        else
        {
            playerNumber.Value = 2;
        }

        Debug.Log(
            $"Player asignado -> ClientId={OwnerClientId} | PlayerNumber={playerNumber.Value}");
    }

    public void AddScore(int amount)
    {
        if (!IsServer)
            return;

        score.Value += amount;

        Debug.Log(
            $"Player {playerNumber.Value} ahora tiene {score.Value} puntos");
    }

    public void ResetScore()
    {
        if (!IsServer)
            return;

        score.Value = 0;
    }
}