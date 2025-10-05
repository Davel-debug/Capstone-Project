using UnityEngine;
using static Pickup;

public class PlayerInventory : MonoBehaviour
{
    public int score = 0;

    private LifeController lifeController;
    private UIController ui;

    // Conteggi per power-up
    private int healthCount = 0;
    private int maxHealthCount = 0;

    void Awake()
    {
        lifeController = GetComponent<LifeController>();
        ui = FindObjectOfType<UIController>();
    }

    public void CollectPickup(PickupType type, int value)
    {
        switch (type)
        {
            case PickupType.Gold:
            case PickupType.Silver:
            case PickupType.Bronze:
                score += value;
                ui?.UpdateScore(score);
                break;

            case PickupType.Health:
                healthCount++;
                lifeController?.Heal(value);
                ui?.ShowPowerUp(type, healthCount);
                break;

            case PickupType.MaxHealth:
                maxHealthCount++;
                lifeController?.IncreaseMaxHealth(value); // usa metodo nel LifeController
                ui?.ShowPowerUp(type, maxHealthCount);
                break;
        }
    }
    public int GetTotalPoints()
    {
        return score;
    }
}
