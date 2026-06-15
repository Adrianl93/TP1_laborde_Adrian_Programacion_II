using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [Header("Match Settings")]
    [SerializeField] private float matchDuration = 120f;
    [SerializeField] private int targetScore = 50;
    [SerializeField] private float countdownDuration = 3f;

    private NetworkVariable<float> remainingTime =
        new NetworkVariable<float>();

    private NetworkVariable<float> countdownTime =
        new NetworkVariable<float>();

    private NetworkVariable<MatchState> matchState =
        new NetworkVariable<MatchState>(
            MatchState.WaitingForPlayers);

    private NetworkVariable<int> winnerPlayerId =
        new NetworkVariable<int>(-1);

    private NetworkVariable<bool> isTie =
        new NetworkVariable<bool>(false);

    public float RemainingTime => remainingTime.Value;

    public float CountdownTime => countdownTime.Value;

    public MatchState CurrentState => matchState.Value;

    public int WinnerPlayerId =>
        winnerPlayerId.Value;

    public bool IsTie =>
        isTie.Value;

    public NetworkVariable<MatchState> MatchStateNetwork =>
        matchState;

    public static MatchManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        remainingTime.Value = matchDuration;

        countdownTime.Value =
            countdownDuration;

        winnerPlayerId.Value = -1;

        isTie.Value = false;

        matchState.Value =
            MatchState.WaitingForPlayers;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        switch (matchState.Value)
        {
            case MatchState.WaitingForPlayers:
                UpdateWaitingPlayers();
                break;

            case MatchState.Countdown:
                UpdateCountdown();
                break;

            case MatchState.Playing:
                UpdatePlaying();
                break;
        }
    }

    private void UpdateWaitingPlayers()
    {
        int connectedPlayers =
            NetworkManager.Singleton
                .ConnectedClientsList.Count;

        if (connectedPlayers >= 2)
        {
            countdownTime.Value =
                countdownDuration;

            matchState.Value =
                MatchState.Countdown;

            Debug.Log(
                "Dos jugadores conectados. Iniciando cuenta regresiva.");
        }
    }

    private void UpdateCountdown()
    {
        countdownTime.Value -=
            Time.deltaTime;

        if (countdownTime.Value <= 0f)
        {
            countdownTime.Value = 0f;

            matchState.Value =
                MatchState.Playing;

            Debug.Log(
                "¡Comienza la partida!");
        }
    }

    private void UpdatePlaying()
    {
        remainingTime.Value -=
            Time.deltaTime;

        if (remainingTime.Value <= 0f)
        {
            remainingTime.Value = 0f;

            EndMatch();
        }
    }

    public bool IsMatchActive()
    {
        return matchState.Value ==
               MatchState.Playing;
    }

    public void CheckScoreVictory(
        ulong playerId,
        int currentScore)
    {
        if (!IsServer)
            return;

        if (matchState.Value !=
            MatchState.Playing)
        {
            return;
        }

        if (currentScore >= targetScore)
        {
            Debug.Log(
                $"Jugador {playerId} alcanzó {targetScore} puntos");

            EndMatch();
        }
    }

    public void RestartMatch()
    {
        if (!IsServer)
            return;

        winnerPlayerId.Value = -1;
        isTie.Value = false;

        remainingTime.Value =
            matchDuration;

        countdownTime.Value =
            countdownDuration;

        ResetPlayers();

        matchState.Value =
            MatchState.Countdown;

        Debug.Log(
            "Partida reiniciada");
    }

    private void ResetPlayers()
    {
        PlayerScore[] scores =
            FindObjectsByType<PlayerScore>(
                FindObjectsSortMode.None);

        foreach (PlayerScore score in scores)
        {
            score.ResetScore();

            PlayerInventory inventory =
                score.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.ResetInventory();
            }

            PlayerStamina stamina =
                score.GetComponent<PlayerStamina>();

            if (stamina != null)
            {
                stamina.ResetStamina();
            }

            Vector3 spawnPosition =
                PlayerSpawnManager.Instance
                    .GetSpawnPosition(
                        score.OwnerClientId);

            CharacterController controller =
                score.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            score.transform.position =
                spawnPosition;

            score.transform.rotation =
                Quaternion.identity;

            if (controller != null)
            {
                controller.enabled = true;
            }
        }
    }

    private void EndMatch()
    {
        if (matchState.Value ==
            MatchState.Finished)
        {
            return;
        }

        AnnounceWinner();

        matchState.Value =
            MatchState.Finished;

        Debug.Log(
            "PARTIDA FINALIZADA");
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
                highestScore =
                    score.Score;

                winner = score;

                tie = false;
            }
            else if (
                score.Score ==
                highestScore)
            {
                tie = true;
            }
        }

        if (tie)
        {
            isTie.Value = true;

            winnerPlayerId.Value = -1;

            Debug.Log(
                "EMPATE");

            return;
        }

        isTie.Value = false;

        if (winner != null)
        {
            winnerPlayerId.Value =
                (int)winner.OwnerClientId;

            Debug.Log(
                $"GANADOR: Jugador {winner.OwnerClientId} con {winner.Score} puntos");
        }
    }
}