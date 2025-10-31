using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExitDoorInventory : Interactable
{
    [System.Serializable]
    public class RequiredItemSlot
    {
        public InventoryItemData itemData;
        public int requiredAmount = 1;

        [HideInInspector] public int currentAmount = 0;

        [Header("UI")]
        public TextMeshProUGUI counterText;   // Mostra oggetti "in possesso/totale da raggiungere"
        public SpriteRenderer iconImage; // Mostra l’icona
    }

    [Header("Required Items")]
    public List<RequiredItemSlot> requiredItems = new List<RequiredItemSlot>();

    [Header("Scene Settings")]
    [Tooltip("Nome della scena vittoria da caricare quando completata.")]
    public string victorySceneName = "Victory";

    [Header("Animation/FX (opzionali)")]
    public Animator animator;
    public string lockedTrigger = "Locked";
    public string openTrigger = "Open";

    private bool allConditionsMet = false;

    private void Start()
    {
        // Inizializza UI
        foreach (var slot in requiredItems)
        {
            if (slot.iconImage != null && slot.itemData != null)
                slot.iconImage.sprite = slot.itemData.icon;

            UpdateSlotUI(slot);
        }

        // Messaggio opzionale
        if (string.IsNullOrEmpty(promptMessage))
            promptMessage = "Premi [E] per inserire oggetti";
    }

    public override void Interact()
    {
        if (allConditionsMet)
        {
            Debug.Log("[ExitDoorInventory] Tutti gli oggetti inseriti. Porta aperta!");
            if (animator != null)
                animator.SetTrigger(openTrigger);

            // Dopo breve delay puoi caricare la scena Victory
            GameManager.Instance.LoadVictoryScreen();
            return;
        }

        // Altrimenti, prova a inserire oggetti dal player
        TryDepositItems();
    }

    private void TryDepositItems()
    {
        InventorySystem playerInventory = InventorySystem.current;
        if (playerInventory == null)
        {
            Debug.LogWarning("[ExitDoorInventory] Nessun inventario player trovato!");
            return;
        }

        bool insertedSomething = false;

        foreach (var slot in requiredItems)
        {
            if (slot.itemData == null) continue;

            // Controlla se il player ha l'oggetto
            InventoryItem playerItem = playerInventory.Get(slot.itemData);
            if (playerItem == null || playerItem.stackSize <= 0)
                continue;

            // Quanti oggetti può trasferire
            int needed = slot.requiredAmount - slot.currentAmount;
            int toTransfer = Mathf.Min(needed, playerItem.stackSize);

            if (toTransfer > 0)
            {
                // Rimuove dal player e aggiunge alla porta
                for (int i = 0; i < toTransfer; i++)
                    playerInventory.Remove(slot.itemData);

                slot.currentAmount += toTransfer;
                UpdateSlotUI(slot);

                Debug.Log($"[ExitDoorInventory] Inseriti {toTransfer}x {slot.itemData.displayName}");
                insertedSomething = true;
            }
        }

        if (insertedSomething)
            CheckAllConditions();
        else
            Debug.Log("[ExitDoorInventory] Nessun oggetto compatibile da inserire.");
    }

    private void CheckAllConditions()
    {
        foreach (var slot in requiredItems)
        {
            if (slot.currentAmount < slot.requiredAmount)
                return; // ancora incompleto
        }

        allConditionsMet = true;
        Debug.Log("[ExitDoorInventory] Tutti gli oggetti inseriti. Porta sbloccata!");
    }

    private void UpdateSlotUI(RequiredItemSlot slot)
    {
        if (slot.counterText != null)
            slot.counterText.text = $"{slot.currentAmount}/{slot.requiredAmount}";

        // Nascondi icona quando completato
        if (slot.iconImage != null)
            slot.iconImage.enabled = slot.currentAmount < slot.requiredAmount;

        if (slot.counterText != null)
            slot.counterText.enabled = slot.currentAmount < slot.requiredAmount;
    }
}
