using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkBootstrap : MonoBehaviour
{
    private void Update()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            if (!NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.StartHost();

                Debug.Log("HOST INICIADO");
            }
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (!NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.StartClient();

                Debug.Log("CLIENTE INICIADO");
            }
        }
    }
}