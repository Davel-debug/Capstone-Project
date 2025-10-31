using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    [Tooltip("Se true, raccoglierlo avvisa i nemici della posizione del player.")]
    public bool alertsEnemies = false;

    public void OnHandlePickupItem()
    {
        InventorySystem.current.Add(referenceItem);

        if (alertsEnemies && AIManager.Instance != null)
            AIManager.Instance.AlertEnemiesToPlayerPosition();

        Destroy(gameObject);
    }
}
