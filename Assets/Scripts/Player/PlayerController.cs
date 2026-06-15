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
    [SerializeField] private float sprintCostPerSecond = 10f;
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
            $"Player spawn | ClientId={OwnerClientId} | IsOwner={IsOwner} | Escena={gameObject.scene.name}");

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

        transform.position =
            PlayerSpawnManager.Instance
                .GetSpawnPosition(
                    OwnerClientId);

        Debug.Log(
            $"Jugador {OwnerClientId} movido al spawn correcto.");
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

        bool canSprint =
            sprintPressed &&
            stamina.HasEnough(
                sprintCostPerSecond * Time.deltaTime);

        movement.Move(
            moveInput,
            canSprint);

        if (canSprint)
        {
            stamina.Consume(
                sprintCostPerSecond * Time.deltaTime);
        }
        else
        {
            stamina.Regenerate(
                staminaRegenPerSecond);
        }

        if (dashPressed)
        {
            dashPressed = false;

            if (dash.TryDash(stamina))
            {
                Debug.Log("Dash ejecutado");
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        sprintPressed = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        sprintPressed = false;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
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