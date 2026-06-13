using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(
            Vector3.forward,
            rotationSpeed * Time.deltaTime,
            Space.World);
    }
}