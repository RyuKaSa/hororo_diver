using System.Collections.Generic;
using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryDisplay inventoryDisplay;
    private Inventory inventory;
    private bool isInventoryOpen = false;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        // V�rifie si la touche A est press�e
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Bascule la visibilit� du panel
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            List<Upgrade> upgrades = GenerateRandomUpgrades();
            if (inventoryDisplay != null)
            {
                inventoryDisplay.DisplayUpgrades(upgrades, inventory);
            }
        }

        inventoryPanel.SetActive(isInventoryOpen);
    }

    public List<Upgrade> GenerateRandomUpgrades()
    {
        string[] attributes = { "damage", "miningSpeed", "resistance", "speed" };
        List<Upgrade> upgrades = new List<Upgrade>();

        for (int i = 0; i < 3; i++)
        {
            string randomAttribute = attributes[Random.Range(0, attributes.Length)];
            float randomPercentage = Random.Range(5f, 20f);
            Dictionary<string, int> requiredOres = new Dictionary<string, int>
            {
                { "IronOre", Random.Range(1, 5) },
                { "GoldOre", Random.Range(0, 3) }
            };

            upgrades.Add(new Upgrade(randomAttribute, randomPercentage, requiredOres));
        }

        return upgrades;
    }

    public void DisplayUpgrades(List<Upgrade> upgrades, Inventory inventory)
    {
        foreach (Upgrade upgrade in upgrades)
        {
            Debug.Log($"Am�lioration : {upgrade.Attribute} +{upgrade.Percentage}%");
        }
    }
}