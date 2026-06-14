using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_Text localIpText;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button exitButton;

    [Header("Network")]
    [SerializeField] private ushort port = 7777;

    private void Start()
    {
        localIpText.text =
            $"Tu IP: {GetLocalIPAddress()}";

        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDestroy()
    {
        if (hostButton != null)
            hostButton.onClick.RemoveListener(StartHost);

        if (clientButton != null)
            clientButton.onClick.RemoveListener(StartClient);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(ExitGame);
    }

    private void StartHost()
    {
        UnityTransport transport =
            NetworkManager.Singleton
                .GetComponent<UnityTransport>();

        transport.SetConnectionData(
            "0.0.0.0",
            port);

        bool started =
            NetworkManager.Singleton.StartHost();

        if (!started)
        {
            Debug.LogError(
                "No se pudo iniciar Host");
            return;
        }

        Debug.Log(
            $"HOST iniciado en puerto {port}");

        NetworkManager.Singleton.SceneManager
            .LoadScene(
                "GameScene",
                LoadSceneMode.Single);
    }

    private void StartClient()
    {
        string ip =
            ipInputField.text.Trim();

        if (string.IsNullOrWhiteSpace(ip))
        {
            Debug.LogWarning(
                "Debe ingresar una IP.");
            return;
        }

        UnityTransport transport =
            NetworkManager.Singleton
                .GetComponent<UnityTransport>();

        transport.SetConnectionData(
            ip,
            port);

        bool started =
            NetworkManager.Singleton.StartClient();

        if (!started)
        {
            Debug.LogError(
                "No se pudo iniciar Cliente");
            return;
        }

        Debug.Log(
            $"Conectando a {ip}:{port}");
    }

    private void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private string GetLocalIPAddress()
    {
        try
        {
            IPHostEntry host =
                Dns.GetHostEntry(
                    Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily ==
                    AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch
        {
            Debug.LogWarning(
                "No se pudo obtener la IP local.");
        }

        return "No encontrada";
    }
}