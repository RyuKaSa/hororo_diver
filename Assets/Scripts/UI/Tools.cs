using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tools Item Data")]
public class Tools : ItemData, IUsable, IEquipable, IDurable
{
    [SerializeField] private int durability;

    void IDurable.OnBreak(InventoryContext _ctx)
    {
        Debug.Log("Break");
    }

    void IDurable.OnRepair(InventoryContext _ctx)
    {
        Debug.Log("Repair");
    }

    void IEquipable.OnEquiped(InventoryContext _ctx)
    {
        Debug.Log("Equiped");
    }

    void IUsable.OnUsed(InventoryContext _ctx)
    {
        Debug.Log("Attack");
    }

    public int MaxDurability => durability;
}
