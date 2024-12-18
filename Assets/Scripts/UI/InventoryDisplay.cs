using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    private int draggedSlotIndex;

    [SerializeField] private InventoryContextMenu contextMenu;
    private Slot[] slots;
    private Inventory inventory;

    public int Initialize(Inventory _inventory)
    {
        slots = GetComponentsInChildren<Slot>();
        inventory = _inventory;

        contextMenu.Init(_inventory);

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
        contextMenu.Select(_index, slots[_index]);
    }

    public void DragSlot(int _index) => draggedSlotIndex = _index;

    public void DropOnSlot(int _index)
    {
        inventory.SwapSlot(draggedSlotIndex, _index);
    }

    #endregion
}
