using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashCooldown = 5f;
    [SerializeField] private float dashCost = 40f;

    private float lastDashTime = -999f;

    public bool CanDash(PlayerStamina stamina)
    {
        bool cooldownReady =
            Time.time >= lastDashTime + dashCooldown;

        bool enoughStamina =
            stamina.HasEnough(dashCost);

        return cooldownReady && enoughStamina;
    }

    public bool TryDash(PlayerStamina stamina)
    {
        if (!CanDash(stamina))
            return false;

        stamina.Consume(dashCost);

        lastDashTime = Time.time;

        return true;
    }
}