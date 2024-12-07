using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    private int draggedSlotIndex; 
    private Slot[] slots;
    private Inventory inventory;

    public int Initialize(Inventory _inventory)
    {
        slots = GetComponentsInChildren<Slot>();
        inventory = _inventory;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Initialize(this, i);
        }

        return slots.Length;
    }

    public void UpdateDisplay(Item[] _items)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateDisplay(_items[i]);
        }
    }

    #region Inputs
    public void ClickSlot(int _index)
    {
        Debug.Log($"Click on slot : {_index}");
    }

    public void DragSlot(int _index) => draggedSlotIndex = _index;

    public void DropOnSlot(int _index)
    {
        inventory.SwapSlot(draggedSlotIndex, _index);
    }

    #endregion
}
