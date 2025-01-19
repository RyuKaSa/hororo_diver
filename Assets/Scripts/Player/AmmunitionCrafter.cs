using UnityEngine;

public class AmmunitionCrafter : MonoBehaviour
{
    private readonly Item[] inventory;

    public AmmunitionCrafter(Item[] inventory)
    {
        this.inventory = inventory;
    }

    public bool CanCraftAmmunition(AmmunitionData ammoData, int batchIndex)
    {
        if (ammoData == null || batchIndex < 0 || batchIndex >= ammoData.CraftBatches.Length)
            return false;

        var batch = ammoData.CraftBatches[batchIndex];
        float multiplier = batch.amount * (1f - batch.discount);

        foreach (var requiredOre in ammoData.RequiredOres)
        {
            int requiredAmount = Mathf.CeilToInt(requiredOre.baseAmount * multiplier);
            int availableAmount = 0;

            foreach (var item in inventory)
            {
                if (item.Data == requiredOre.ore)
                {
                    availableAmount += item.Count;
                }
            }

            if (availableAmount < requiredAmount)
            {
                return false;
            }
        }
        return true;
    }

    public bool CraftAmmunition(AmmunitionData ammoData, int batchIndex)
    {
        if (!CanCraftAmmunition(ammoData, batchIndex))
        {
            Debug.LogWarning($"Cannot craft {ammoData.itemName}: insufficient resources");
            return false;
        }

        var batch = ammoData.CraftBatches[batchIndex];
        float multiplier = batch.amount * (1f - batch.discount);

        // Consommer les ressources
        foreach (var requiredOre in ammoData.RequiredOres)
        {
            int requiredAmount = Mathf.CeilToInt(requiredOre.baseAmount * multiplier);
            int remainingToConsume = requiredAmount;

            for (int i = 0; i < inventory.Length && remainingToConsume > 0; i++)
            {
                if (inventory[i].Data == requiredOre.ore)
                {
                    int amountToConsume = Mathf.Min(remainingToConsume, inventory[i].Count);
                    inventory[i] = new Item(inventory[i].Data, inventory[i].Count - amountToConsume);
                    remainingToConsume -= amountToConsume;

                    if (inventory[i].Count == 0)
                    {
                        inventory[i] = new Item();
                    }
                }
            }

            if (remainingToConsume > 0)
            {
                Debug.LogError($"Error during crafting: not enough {requiredOre.ore.itemName}");
                return false;
            }
        }

        // Ajouter les munitions craftées
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].Empty)
            {
                inventory[i] = new Item(ammoData, batch.amount);
                Debug.Log($"Successfully crafted {batch.amount} {ammoData.itemName}");
                return true;
            }
            else if (inventory[i].Data == ammoData &&
                     inventory[i].Count + batch.amount <= ammoData.stackMaxCount)
            {
                inventory[i] = new Item(ammoData, inventory[i].Count + batch.amount);
                Debug.Log($"Successfully crafted {batch.amount} {ammoData.itemName}");
                return true;
            }
        }

        Debug.LogWarning("Inventory is full, cannot add crafted ammunition");
        return false;
    }
}