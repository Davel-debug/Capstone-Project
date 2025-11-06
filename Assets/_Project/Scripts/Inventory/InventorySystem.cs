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

    [Header("Inventory Limits")]
    public int maxSlots = 3;


    public event System.Action OnInventoryChanged; //Evento notifica UI

    private void Awake()
    {
        current = this;
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        // Prima controlla il dizionario (oggetti stackabili)
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
            return value;

        // Poi cerca nella lista (oggetti non stackabili)
        foreach (var item in inventory)
        {
            if (item.data == referenceData)
                return item;
        }

        return null;
    }


    public bool Add(InventoryItemData referenceData)
    {
        // Controllo numero massimo slot
        if (inventory.Count >= maxSlots)
        {
            Debug.LogWarning("Inventario pieno! Non puoi raccogliere altri oggetti.");
            return false;
        }

        if (!referenceData.isStackable)
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            OnInventoryChanged?.Invoke();
            return true;
        }

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

        OnInventoryChanged?.Invoke(); // Notifica UI
        return true;
    }

    public void Remove(InventoryItemData referenceData)
    {
        InventoryItem itemToRemove = null;

        // Se esiste nel dizionario è un oggetto stackabile
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if (value.stackSize <= 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
                itemToRemove = value;
            }
        }
        else
        {
            // Cerca nella lista (per oggetti non stackabili)
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].data == referenceData)
                {
                    itemToRemove = inventory[i];
                    inventory.RemoveAt(i);
                    break;
                }
            }
        }

        if (itemToRemove != null)
            Debug.Log($"Rimosso: {referenceData.displayName}");

        OnInventoryChanged?.Invoke(); // Aggiorna UI
    }

    // EQUIP
    public void EquipItem(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            Debug.LogWarning("Indice inventario non valido.");
            return;
        }

        if (index == equippedIndex)
        {
            UnequipItem();
            return;
        }

        // Distruggi quello attuale
        if (equippedObject != null)
        {
            Destroy(equippedObject);
        }

        // Istanzia nuovo oggetto in mano
        InventoryItem item = inventory[index];
        equippedObject = Instantiate(item.data.prefab, handSlot);
        equippedObject.transform.localPosition = Vector3.zero;
        equippedObject.transform.localRotation = Quaternion.identity;

        // Rimozione RigidBody e Collider
        var equipScript = equippedObject.GetComponent<EquipableItem>();
        if (equipScript != null)
            equipScript.OnEquipped();

        equippedIndex = index;
        Debug.Log($"Equipaggiato: {item.data.displayName}");

        OnInventoryChanged?.Invoke();
    }
    // UNEQUIP
    public void UnequipItem()
    {
        if (equippedObject != null)
        {
            Destroy(equippedObject);
            equippedObject = null;
        }

        Debug.Log("Oggetto disequipaggiato.");

        equippedIndex = -1;
        OnInventoryChanged?.Invoke();
    }

    // DROP
    public void DropItem()
    {
        if (equippedIndex < 0 || equippedIndex >= inventory.Count)
            return;

        InventoryItem item = inventory[equippedIndex];

        // Distruggi modello in mano
        if (equippedObject != null)
        {
            Destroy(equippedObject);
        }

        // Istanzia a terra il prefab originale
        GameObject dropped = Instantiate(item.data.prefab);
        dropped.transform.position = transform.position + transform.forward * 1.5f;
        dropped.transform.rotation = Quaternion.identity;

        // Aggiunta RigidBody e Collider
        var equipScript = dropped.GetComponent<EquipableItem>();
        if (equipScript != null)
            equipScript.OnDropped();

        Debug.Log($"Droppato: {item.data.displayName}");

        Remove(item.data);

        if (inventory.Count == 0)
        {
            equippedIndex = -1;
        }
        else if (equippedIndex >= inventory.Count)
        {
            equippedIndex = inventory.Count - 1;
            EquipItem(equippedIndex);
        }


        OnInventoryChanged?.Invoke();
    }
    public int GetEquippedIndex()
    {
        return equippedIndex;
    }

    public bool TransferItemTo(InventorySystem target, InventoryItemData data, int amount = 1)
    {
        InventoryItem item = Get(data);
        if (item == null || item.stackSize < amount)
            return false;

        for (int i = 0; i < amount; i++)
            Remove(data);

        target.Add(data);

        OnInventoryChanged?.Invoke();

        return true;
    }

}
