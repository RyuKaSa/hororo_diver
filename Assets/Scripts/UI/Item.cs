using UnityEngine;

[System.Serializable]
public struct Item
{
    public ItemData Data { get; private set; }
    public int count;

    public int Count
    {
        get => count;
        private set => count = value;
    }

    public string Name => Data?.name ?? "";
    public int Quantity => Count;

    // Constructeur par défaut
    public Item(ItemData data = null, int count = 1)
    {
        Data = data;
        this.count = count;
    }

    public Item ModifyCount(int amount)
    {
        return new Item(Data, Mathf.Max(0, count + amount));
    }

    // Propriété pour vérifier si l'item est vide
    public bool Empty => Data == null || Count <= 0;

    // Vérifie si l'emplacement peut accueillir l'item
    public bool AvailableFor(Item itemToAdd)
    {
        if (Empty) return true;
        if (Data != itemToAdd.Data) return false;
        return Count + itemToAdd.Count <= Data.stackMaxCount;
    }

    // Fusionne avec un autre item
    public void Merge(ref Item itemToAdd)
    {
        if (!AvailableFor(itemToAdd)) return;

        if (Empty)
        {
            Data = itemToAdd.Data;
            Count = itemToAdd.Count;
            itemToAdd = new Item();
            return;
        }

        int spaceAvailable = Data.stackMaxCount - Count;
        int amountToAdd = Mathf.Min(spaceAvailable, itemToAdd.Count);

        Count += amountToAdd;
        itemToAdd = new Item(itemToAdd.Data, itemToAdd.Count - amountToAdd);
    }
}

public class AmmunitionManager : MonoBehaviour
{
    private Item[] inventoryItems;
    private AmmunitionCrafter crafter;

    private void Awake()
    {
        inventoryItems = GetComponent<Inventory>().Data;
        crafter = new AmmunitionCrafter(inventoryItems);
    }

    public bool CraftAmmunition(AmmunitionData ammoData, int batchIndex)
    {
        return crafter.CraftAmmunition(ammoData, batchIndex);
    }

    public bool CanCraftAmmunition(AmmunitionData ammoData, int batchIndex)
    {
        return crafter.CanCraftAmmunition(ammoData, batchIndex);
    }
}