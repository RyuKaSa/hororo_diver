using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryDisplay display;
    private InventoryData data;

    private void Awake()
    {
        int _slotCount  = display.Initialize();
        data = new InventoryData(_slotCount);
    }
    
    public Item AddItem(Item _item)
    {
        if (!data.SlotAvailable(_item)) return _item;

        _item = data.AddItem(_item);

        display.UpdateDisplay(data.items);

        return _item;
    }

    public Item PickItem(int _slotID)
    {
        Item _result = data.Pick(_slotID);

        display.UpdateDisplay(data.items);

        return _result;
    }
}
