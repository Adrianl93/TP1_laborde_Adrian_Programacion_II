using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [Header("Match Settings")]
    [SerializeField] private float matchDuration = 120f;
    [SerializeField] private int targetScore = 50;

    private NetworkVariable<float> remainingTime =
        new NetworkVariable<float>();

    private NetworkVariable<bool> matchEnded =
        new NetworkVariable<bool>(false);

    public float RemainingTime => remainingTime.Value;
    public bool MatchEnded => matchEnded.Value;

    public static MatchManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            remainingTime.Value = matchDuration;
        }
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (matchEnded.Value)
            return;

        remainingTime.Value -= Time.deltaTime;

        if (remainingTime.Value <= 0f)
        {
            remainingTime.Value = 0f;
            EndMatch();
        }
    }

    public void CheckScoreVictory(
        ulong playerId,
        int currentScore)
    {
        if (!IsServer)
            return;

        if (matchEnded.Value)
            return;

        if (currentScore >= targetScore)
        {
            Debug.Log(
                $"Jugador {playerId} alcanzó {targetScore} puntos");

            EndMatch();
        }
    }

    private void EndMatch()
    {
        matchEnded.Value = true;

        Debug.Log("PARTIDA FINALIZADA");

        AnnounceWinner();
    }

    private void AnnounceWinner()
    {
        PlayerScore[] scores =
            FindObjectsByType<PlayerScore>(
                FindObjectsSortMode.None);

        PlayerScore winner = null;
        int highestScore = -1;
        bool tie = false;

        foreach (PlayerScore score in scores)
        {
            if (score.Score > highestScore)
            {
                highestScore = score.Score;
                winner = score;
                tie = false;
            }
            else if (score.Score == highestScore)
            {
                tie = true;
            }
        }

        if (tie)
        {
            Debug.Log("EMPATE");
            return;
        }

        if (winner != null)
        {
            Debug.Log(
                $"GANADOR: Jugador {winner.OwnerClientId} con {winner.Score} puntos");
        }
    }
}