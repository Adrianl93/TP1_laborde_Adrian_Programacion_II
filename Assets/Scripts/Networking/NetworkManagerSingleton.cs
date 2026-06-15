using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSingleton : MonoBehaviour
{
    private static NetworkManagerSingleton instance;

    private void Awake()
    {
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        ShutdownNetwork();
    }

    private void OnDestroy()
    {
        ShutdownNetwork();
    }

    private void ShutdownNetwork()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            Debug.Log(
                "Cerrando NetworkManager");

            NetworkManager.Singleton.Shutdown();
        }
    }
}