using System;

public class InventoryData
{
    public InventoryData(int _slotCount)
    {
        items = new Item[_slotCount];
    }

    public Item[] items { private set; get; }

    public bool SlotAvailable(Item _itemToAdd)
    {
        foreach (var _item in items)
        {
            if (_item.AvailableFor(_itemToAdd)) return true;
        }
        return false;
    }

    public void AddItem(ref Item _itemToAdd)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (_itemToAdd.Empty) return;

            if (items[i].AvailableFor(_itemToAdd))
            {
                items[i].Merge(ref _itemToAdd);
            }
        }
    }

    public Item Pick(int _slotID)
    {
        if (_slotID > items.Length) throw new SystemException($"Id {_slotID} out of Inventory");

        Item _item = items[_slotID];
        items[_slotID] = new Item();

        return _item;
    }

    public void Swap(int _slotA, int _slotB)
    {
        Item _temp = items[_slotA];
        items[_slotA] = items[_slotB];
        items[_slotB] = _temp;
    }
}