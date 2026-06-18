using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerController : NetworkBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float sprintCostPerSecond = 20f;

    [SerializeField] private float staminaRegenPerSecond = 15f;

    private PlayerInputActions inputActions;

    private PlayerMovement movement;
    private PlayerStamina stamina;
    private PlayerDash dash;

    private Vector2 moveInput;

    private bool sprintPressed;
    private bool dashPressed;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stamina = GetComponent<PlayerStamina>();
        dash = GetComponent<PlayerDash>();

        inputActions = new PlayerInputActions();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(
            $"Player spawn | ClientId={OwnerClientId} | IsOwner={IsOwner} | IsHost={IsHost}");

        StartCoroutine(
            WaitForSpawnManager());

        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Sprint.performed += OnSprintPerformed;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;

        inputActions.Player.Dash.performed += OnDashPerformed;
    }

    private IEnumerator WaitForSpawnManager()
    {
        while (PlayerSpawnManager.Instance == null)
        {
            yield return null;
        }

        Vector3 spawnPosition =
            PlayerSpawnManager.Instance
                .GetSpawnPosition(
                    OwnerClientId);

        CharacterController controller =
            GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = spawnPosition;
        transform.rotation = Quaternion.identity;

        if (controller != null)
        {
            controller.enabled = true;
        }

        stamina.ResetStamina();

        Debug.Log(
            $"Jugador {OwnerClientId} movido al spawn correcto.");
    }

    [ClientRpc]
    public void TeleportClientRpc(
        Vector3 position)
    {
        Debug.Log(
            $"TeleportClientRpc recibido en Player {OwnerClientId}");

        CharacterController controller =
            GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = position;
        transform.rotation = Quaternion.identity;

        if (controller != null)
        {
            controller.enabled = true;
        }

        stamina.ResetStamina();

        Debug.Log(
            $"Teleport aplicado en client {OwnerClientId} -> {position}");
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Sprint.performed -= OnSprintPerformed;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;

        inputActions.Player.Dash.performed -= OnDashPerformed;

        inputActions.Disable();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (MatchManager.Instance == null)
            return;

        if (!MatchManager.Instance.IsMatchActive())
            return;

        bool sprinting = false;

        if (sprintPressed)
        {
            sprinting =
                stamina.Consume(
                    sprintCostPerSecond * Time.deltaTime);
        }
        else
        {
            stamina.Regenerate(
                staminaRegenPerSecond);
        }

        movement.Move(
            moveInput,
            sprinting);

        if (dashPressed)
        {
            dashPressed = false;

            Vector3 dashDirection =
            new Vector3(
             moveInput.x,
             0f,
            moveInput.y);

            if (dash.TryDash(
                    stamina,
                    dashDirection))
            {
                Debug.Log(
                    $"Dash ejecutado | Player {OwnerClientId}");
            }
        }
    }

    private void OnMove(
        InputAction.CallbackContext context)
    {
        moveInput =
            context.ReadValue<Vector2>();
    }

    private void OnSprintPerformed(
        InputAction.CallbackContext context)
    {
        sprintPressed = true;
    }

    private void OnSprintCanceled(
        InputAction.CallbackContext context)
    {
        sprintPressed = false;
    }

    private void OnDashPerformed(
        InputAction.CallbackContext context)
    {
        dashPressed = true;
    }

    public override void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;

            inputActions.Player.Sprint.performed -= OnSprintPerformed;
            inputActions.Player.Sprint.canceled -= OnSprintCanceled;

            inputActions.Player.Dash.performed -= OnDashPerformed;

            inputActions.Player.Disable();
            inputActions.UI.Disable();
            inputActions.Disable();
        }

        base.OnDestroy();
    }
}