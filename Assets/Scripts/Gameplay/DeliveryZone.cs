using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró a DeliveryZone");
        PlayerInventory inventory =
            other.GetComponent<PlayerInventory>();

        PlayerScore score =
            other.GetComponent<PlayerScore>();

        if (inventory == null || score == null)
            return;

        Debug.Log($"HasItem: {inventory.HasItem}");
        int deliveredValue =
            inventory.DeliverItem();

        if (deliveredValue <= 0)
            return;

        score.AddPoints(deliveredValue);

        Debug.Log(
            $"Objeto entregado. Valor: {deliveredValue}");
    }
}