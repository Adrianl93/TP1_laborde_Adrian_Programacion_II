using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text hostIPText;
    [SerializeField] private TMP_Text scoreP1Text;
    [SerializeField] private TMP_Text scoreP2Text;

    [Header("Bars")]
    [SerializeField] private Slider staminaBar;

    private PlayerStamina localPlayerStamina;

    private bool searchingPlayer;

    private void Start()
    {
        UpdateHostIP();
    }

    private void Update()
    {
        if (localPlayerStamina == null &&
            !searchingPlayer)
        {
            FindLocalPlayer();
        }

        UpdateTimer();
        UpdateScores();
        UpdateStamina();
    }

    private void FindLocalPlayer()
    {
        searchingPlayer = true;

        NetworkObject[] players =
            FindObjectsByType<NetworkObject>(
                FindObjectsSortMode.None);

        foreach (NetworkObject player in players)
        {
            if (!player.IsOwner)
                continue;

            PlayerStamina stamina =
                player.GetComponent<PlayerStamina>();

            if (stamina == null)
                continue;

            localPlayerStamina = stamina;

            staminaBar.minValue = 0f;

            staminaBar.maxValue =
                localPlayerStamina.MaxStamina;

            staminaBar.value =
                localPlayerStamina.CurrentStamina;

            Debug.Log(
                $"HUD asignado a Player {player.OwnerClientId}");

            return;
        }

        searchingPlayer = false;
    }

    private void UpdateTimer()
    {
        if (MatchManager.Instance == null)
            return;

        float time =
            MatchManager.Instance.RemainingTime;

        int minutes =
            Mathf.FloorToInt(time / 60);

        int seconds =
            Mathf.FloorToInt(time % 60);

        timeText.text =
            $"Quedan {minutes:00}:{seconds:00}";
    }

    private void UpdateScores()
    {
        PlayerScore[] scores =
            FindObjectsByType<PlayerScore>(
                FindObjectsSortMode.None);

        scoreP1Text.text =
            "P1: 0 pts";

        scoreP2Text.text =
            "P2: 0 pts";

        foreach (PlayerScore playerScore in scores)
        {
            if (playerScore.PlayerNumber == 1)
            {
                scoreP1Text.text =
                    $"P1: {playerScore.Score} pts";
            }
            else if (playerScore.PlayerNumber == 2)
            {
                scoreP2Text.text =
                    $"P2: {playerScore.Score} pts";
            }
        }
    }

    private void UpdateStamina()
    {
        if (localPlayerStamina == null)
            return;

        staminaBar.maxValue =
            localPlayerStamina.MaxStamina;

        staminaBar.value =
            localPlayerStamina.CurrentStamina;
    }

    private void UpdateHostIP()
    {
        string localIP = "No IP";

        try
        {
            var host =
                Dns.GetHostEntry(
                    Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily ==
                    AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch
        {
            localIP = "IP Error";
        }

        hostIPText.text =
            $"Host: {localIP}:7777";
    }
}