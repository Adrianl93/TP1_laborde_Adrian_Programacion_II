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

    private void Start()
    {
        FindLocalPlayer();
    }

    private void Update()
    {
        UpdateTimer();
        UpdateScores();
        UpdateStamina();
    }

    private void FindLocalPlayer()
    {
        NetworkObject[] players =
            FindObjectsByType<NetworkObject>(
                FindObjectsSortMode.None);

        foreach (NetworkObject player in players)
        {
            if (player.IsOwner)
            {
                localPlayerStamina =
                    player.GetComponent<PlayerStamina>();

                if (localPlayerStamina != null)
                {
                    staminaBar.minValue = 0f;
                    staminaBar.maxValue =
                        localPlayerStamina.MaxStamina;
                }

                break;
            }
        }
    }

    private void UpdateTimer()
    {
        if (MatchManager.Instance == null)
            return;

        float time =
            MatchManager.Instance.RemainingTime;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timeText.text =
            $"{minutes:00}:{seconds:00}";
    }

    private void UpdateScores()
    {
        PlayerScore[] scores =
            FindObjectsByType<PlayerScore>(
                FindObjectsSortMode.None);

        foreach (PlayerScore playerScore in scores)
        {
            if (playerScore.PlayerId == 0)
            {
                scoreP1Text.text =
                    playerScore.Score.ToString();
            }
            else if (playerScore.PlayerId == 1)
            {
                scoreP2Text.text =
                    playerScore.Score.ToString();
            }
        }
    }

    private void UpdateStamina()
    {
        if (localPlayerStamina == null)
            return;

        staminaBar.value =
            localPlayerStamina.CurrentStamina /
            localPlayerStamina.MaxStamina;
    }

    public void SetHostIP(string ip)
    {
        hostIPText.text = ip;
    }
}