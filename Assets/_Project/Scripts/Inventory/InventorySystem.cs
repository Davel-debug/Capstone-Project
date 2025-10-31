using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem current;
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory { get; private set; }

    [Header("Equip Settings")]
    public Transform handSlot;
    private GameObject equippedObject;
    private int equippedIndex = -1;

    private void Awake()
    {
        current = this;
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }
    }

    public void Remove(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if(value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }
        
    }
    // EQUIP
    public void EquipItem(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            Debug.LogWarning("Indice inventario non valido.");
            return;
        }

        // distruggi quello attuale
        if (equippedObject != null)
        {
            Destroy(equippedObject);
        }

        // istanzia nuovo oggetto in mano
        InventoryItem item = inventory[index];
        equippedObject = Instantiate(item.data.prefab, handSlot);
        equippedObject.transform.localPosition = Vector3.zero;
        equippedObject.transform.localRotation = Quaternion.identity;

        equippedIndex = index;
        Debug.Log($"Equipaggiato: {item.data.displayName}");
    }

    // DROP
    public void DropItem()
    {
        if (equippedIndex < 0 || equippedIndex >= inventory.Count)
            return;

        InventoryItem item = inventory[equippedIndex];

        // distruggi modello in mano
        if (equippedObject != null)
        {
            Destroy(equippedObject);
        }

        // istanzia a terra il prefab originale
        GameObject dropped = Instantiate(item.data.prefab);
        dropped.transform.position = transform.position + transform.forward * 1.5f;
        dropped.transform.rotation = Quaternion.identity;

        Debug.Log($"Droppato: {item.data.displayName}");

        Remove(item.data);
        equippedIndex = inventory.Count > 0 ? 0 : -1;

        if (equippedIndex >= 0)
            EquipItem(equippedIndex);
    }
}
