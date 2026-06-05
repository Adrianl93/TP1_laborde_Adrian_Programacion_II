using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 15f;

    private float currentStamina;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        Regenerate();
    }

    private void Regenerate()
    {
        currentStamina += regenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public bool HasEnough(float amount)
    {
        return currentStamina >= amount;
    }

    public bool Consume(float amount)
    {
        if (!HasEnough(amount))
            return false;

        currentStamina -= amount;
        return true;
    }
}