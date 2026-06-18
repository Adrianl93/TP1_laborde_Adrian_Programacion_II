using TMPro;
using Unity.Netcode;
using UnityEngine;
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
        if (newState == MatchState.Countdown)
        {
            panel.SetActive(false);
            return;
        }

        if (newState != MatchState.Finished)
            return;

        StartCoroutine(
            WaitForResultsSync());
    }

    private void ShowResults()
    {
        panel.SetActive(true);

        replayButton.interactable =
            NetworkManager.Singleton.IsHost;

        PlayerScore[] scores =
            FindObjectsByType<PlayerScore>(
                FindObjectsSortMode.None);

        foreach (PlayerScore score in scores)
        {
            Debug.Log(
                $"UI SCORE -> Player {score.PlayerNumber} = {score.Score}");

            if (score.PlayerNumber == 1)
            {
                scoreP1Text.text =
                    $"Score: {score.Score}";
            }
            else if (score.PlayerNumber == 2)
            {
                scoreP2Text.text =
                    $"Score: {score.Score}";
            }
        }

        Debug.Log(
            $"UI RESULT -> Tie={MatchManager.Instance.IsTie} | Winner={MatchManager.Instance.WinnerPlayerNumber}");

        if (MatchManager.Instance.IsTie)
        {
            winnerText.text =
                "It's a Tie";
        }
        else
        {
            winnerText.text =
                $"Winner: Player {MatchManager.Instance.WinnerPlayerNumber}";
        }
    }

    private void ReplayMatch()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        panel.SetActive(false);

        winnerText.text = "";
        scoreP1Text.text = "";
        scoreP2Text.text = "";

        StopAllCoroutines();

        MatchManager.Instance
            .RestartMatch();
    }

    private System.Collections.IEnumerator WaitForResultsSync()
    {
        Debug.Log(
            "Esperando sincronización de resultados...");

        while (
            !MatchManager.Instance.IsTie &&
            MatchManager.Instance.WinnerPlayerNumber == -1)
        {
            yield return null;
        }

        Debug.Log(
            $"Resultados sincronizados | Winner={MatchManager.Instance.WinnerPlayerNumber} | Tie={MatchManager.Instance.IsTie}");

        ShowResults();
    }

    private void ReturnToMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton
                .Shutdown();
        }

        UnityEngine.SceneManagement.SceneManager
            .LoadScene("MainMenu");
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