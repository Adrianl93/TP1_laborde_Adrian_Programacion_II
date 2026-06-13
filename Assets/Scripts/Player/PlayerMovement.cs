using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.75f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sprintCostPerSecond = 10f;
    [SerializeField] private float staminaRegenPerSecond = 15f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input, bool sprinting)
    {
        float speed = moveSpeed;

        if (sprinting)
            speed *= sprintMultiplier;

        Vector3 moveDirection =
            new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        controller.Move(
            moveDirection.normalized *
            speed *
            Time.deltaTime
        );
    }
}