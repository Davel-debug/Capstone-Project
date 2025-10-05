using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Gold, Silver, Bronze, Health, MaxHealth }
    public PickupType pickupType;

    [SerializeField] private int value;  // valore impostato automaticamente

    private void Reset()
    {
        SetDefaultValue();
    }

    private void OnValidate()
    {
        SetDefaultValue();
    }

    private void SetDefaultValue()
    {
        switch (pickupType)
        {
            case PickupType.Gold:
                value = 10;
                break;
            case PickupType.Silver:
                value = 5;
                break;
            case PickupType.Bronze:
                value = 1;
                break;
            case PickupType.Health:
                value = 20;
                break;
            case PickupType.MaxHealth:
                value = 10;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            Debug.Log("Inventario trovato");
            inventory.CollectPickup(pickupType, value);
            Destroy(gameObject);
        }
        Debug.Log("Inventario non trovato ");
    }
}
