using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    private Slot[] slots;
    public int Initialize()
    {
        slots = GetComponentsInChildren<Slot>();

        return slots.Length;
    }

    public void UpdateDisplay(Item[] _data)
    {

    }
}
