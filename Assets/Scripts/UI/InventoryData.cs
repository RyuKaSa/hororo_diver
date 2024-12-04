[System.Serializable]
public class InventoryData
{
    public InventoryData(int _slotCount)
    {
        items = new Item[_slotCount];
    }

    public Item[] items { private set; get; }

    public bool SlotAvailable(Item _itemToAdd)
    {
        throw new System.NotImplementedException();
    }

    public Item AddItem(Item _itemToAdd)
    {
        throw new System.NotImplementedException();
    }

    public Item Pick(int _slotID)
    {
        throw new System.NotImplementedException();
    }
}