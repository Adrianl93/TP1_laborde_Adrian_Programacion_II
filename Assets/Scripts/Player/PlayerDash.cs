using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : NetworkBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashCooldown = 5f;

    [SerializeField] private float dashCost = 40f;

    [SerializeField] private float dashSpeed = 18f;

    [SerializeField] private float dashDuration = 0.2f;

    [SerializeField] private float hitRadius = 1.5f;

    private float lastDashTime = -999f;

    private bool isDashing;

    private CharacterController controller;

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();
    }

    public bool CanDash(
        PlayerStamina stamina)
    {
        bool cooldownReady =
            Time.time >=
            lastDashTime + dashCooldown;

        bool enoughStamina =
            stamina.HasEnough(dashCost);

        return cooldownReady &&
               enoughStamina &&
               !isDashing;
    }

    public bool TryDash(
        PlayerStamina stamina,
        Vector3 direction)
    {
        if (!CanDash(stamina))
            return false;

        if (direction.sqrMagnitude <= 0.01f)
            return false;

        stamina.Consume(dashCost);

        lastDashTime =
            Time.time;

        direction =
            direction.normalized;

        StartCoroutine(
            DashCoroutine(direction));

        RequestDashHitServerRpc();

        return true;
    }

    private IEnumerator DashCoroutine(
        Vector3 direction)
    {
        isDashing = true;

        float timer = 0f;

        while (timer < dashDuration)
        {
            controller.Move(
                direction *
                dashSpeed *
                Time.deltaTime);

            timer += Time.deltaTime;

            yield return null;
        }

        isDashing = false;
    }

    [ServerRpc]
    private void RequestDashHitServerRpc()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                hitRadius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            PlayerInventory inventory =
                hit.GetComponent<PlayerInventory>();

            if (inventory == null)
                continue;

            if (!inventory.HasItem)
                continue;

            inventory.DropItem();

            Debug.Log(
                $"Dash hit -> Player {OwnerClientId} hizo dropear item");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            hitRadius);
    }
}