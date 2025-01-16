using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Items/Ammunition Data")]
public class AmmunitionData : ItemData
{
    [System.Serializable]
    public struct RequiredOre
    {
        public ItemData ore;
        public int baseAmount;  // Quantit� de base n�cessaire
    }

    [System.Serializable]
    public struct CraftBatch
    {
        public int amount;     // Quantit� produite
        public float discount; // R�duction sur les ressources (0-1)
    }

    [Header("Ammunition Properties")]
    public float damage;
    public RequiredOre[] RequiredOres;
    public CraftBatch[] CraftBatches = new CraftBatch[]
    {
        new CraftBatch { amount = 1, discount = 0f },    // Craft unitaire
        new CraftBatch { amount = 5, discount = 0.1f },  // Craft par 5 avec 10% de r�duction
        new CraftBatch { amount = 25, discount = 0.2f }  // Craft par 25 avec 20% de r�duction
    };
}

public class AmmunitionUI : MonoBehaviour
{
    [SerializeField] private AmmunitionData ammoData;
    [SerializeField] private Inventory inventory;
    private AmmunitionCrafter crafter;

    private void Start()
    {
        crafter = new AmmunitionCrafter(inventory.Data);
    }

    public void OnCraftButtonClick(int batchIndex)
    {
        if (crafter.CanCraftAmmunition(ammoData, batchIndex))
        {
            if (crafter.CraftAmmunition(ammoData, batchIndex))
            {
                // Mise � jour de l'interface utilisateur si n�cessaire
                Debug.Log($"Crafted ammunition batch {batchIndex} successfully");
            }
        }
        else
        {
            Debug.Log("Not enough resources for crafting");
        }
    }
}

// Extension de la classe Item pour supporter les munitions
public static class ItemExtensions
{
    public static bool IsAmmunition(this Item item)
    {
        return item.Data != null && item.Data is AmmunitionData;
    }

    public static AmmunitionData GetAmmunitionData(this Item item)
    {
        return item.Data as AmmunitionData;
    }
}