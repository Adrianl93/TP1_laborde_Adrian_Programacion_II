using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMatchUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Texts")]
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text scoreP1Text;
    [SerializeField] private TMP_Text scoreP2Text;

    [Header("Buttons")]
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        panel.SetActive(false);

        replayButton.onClick.AddListener(
            ReplayMatch);

        menuButton.onClick.AddListener(
            ReturnToMenu);

        exitButton.onClick.AddListener(
            ExitGame);

        RegisterMatchEvents();
    }

    private void OnDestroy()
    {
        replayButton.onClick.RemoveListener(
            ReplayMatch);

        menuButton.onClick.RemoveListener(
            ReturnToMenu);

        exitButton.onClick.RemoveListener(
            ExitGame);

        UnregisterMatchEvents();
    }

    private void RegisterMatchEvents()
    {
        if (MatchManager.Instance == null)
            return;

        MatchManager.Instance
            .MatchStateNetwork
            .OnValueChanged +=
            OnMatchStateChanged;
    }

    private void UnregisterMatchEvents()
    {
        if (MatchManager.Instance == null)
            return;

        MatchManager.Instance
            .MatchStateNetwork
            .OnValueChanged -=
            OnMatchStateChanged;
    }

    private void OnMatchStateChanged(
        MatchState previousState,
        MatchState newState)
    {
        if (newState != MatchState.Finished)
            return;

        ShowResults();
    }

    private void ShowResults()
    {
        panel.SetActive(true);

        PlayerScore[] scores =
            FindObjectsByType<PlayerScore>(
                FindObjectsSortMode.None);

        foreach (PlayerScore score in scores)
        {
            if (score.PlayerId == 0)
            {
                scoreP1Text.text =
                    score.Score.ToString();
            }
            else if (score.PlayerId == 1)
            {
                scoreP2Text.text =
                    score.Score.ToString();
            }
        }

        if (MatchManager.Instance.IsTie)
        {
            winnerText.text =
                "EMPATE";
        }
        else
        {
            winnerText.text =
                $"GANADOR: JUGADOR {MatchManager.Instance.WinnerPlayerId + 1}";
        }
    }

    private void ReplayMatch()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        NetworkManager.Singleton
            .SceneManager
            .LoadScene(
                "GameScene",
                LoadSceneMode.Single);
    }

    private void ReturnToMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton
                .Shutdown();
        }

        SceneManager.LoadScene(
            "MainMenu");
    }

    private void ExitGame()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton
                .Shutdown();
        }

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}