using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("PowerUp UI Slots")]
    public GameObject healthPowerUpSlot;      // GameObject che contiene Image e Text
    public GameObject maxHealthPowerUpSlot;   // Idem

    public Sprite healthSprite;
    public Sprite maxHealthSprite;

    private Dictionary<Pickup.PickupType, PowerUpSlot> powerUpSlots;

    private void Awake()
    {
        powerUpSlots = new Dictionary<Pickup.PickupType, PowerUpSlot>();

        // Setup dizionario e slot disattivati inizialmente
        powerUpSlots[Pickup.PickupType.Health] = new PowerUpSlot(healthPowerUpSlot, healthSprite);
        powerUpSlots[Pickup.PickupType.MaxHealth] = new PowerUpSlot(maxHealthPowerUpSlot, maxHealthSprite);

        foreach (var slot in powerUpSlots.Values)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void UpdateHealth(float current, float max)
    {
        healthBar.value = current / max;
        healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateTimer(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ShowPowerUp(Pickup.PickupType type, int count)
    {
        if (!powerUpSlots.ContainsKey(type))
            return;

        var slot = powerUpSlots[type];
        slot.gameObject.SetActive(true);
        slot.icon.sprite = slot.defaultSprite;
        slot.countText.text = $"{count}";
    }

    public void HidePowerUp(Pickup.PickupType type)
    {
        if (powerUpSlots.ContainsKey(type))
            powerUpSlots[type].gameObject.SetActive(false);
    }

    private class PowerUpSlot
    {
        public GameObject gameObject;
        public Image icon;
        public TextMeshProUGUI countText;
        public Sprite defaultSprite;

        public PowerUpSlot(GameObject slotObj, Sprite defaultSprite)
        {
            gameObject = slotObj;
            this.defaultSprite = defaultSprite;
            icon = slotObj.GetComponentInChildren<Image>();
            countText = slotObj.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
