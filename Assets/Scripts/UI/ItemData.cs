using UnityEngine;

[CreateAssetMenu(menuName ="Items/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] public string itemName;

    [SerializeField] public int stackMaxCount = 1;

    [SerializeField] public Sprite icon;
}

public interface IConsumable
{
    void OnConsumed(InventoryContext _ctx);
}

public interface IUsable
{
    void OnUsed(InventoryContext _ctx);
}

public interface IDurable
{
    int MaxDurability { get; }

    void OnBreak(InventoryContext _ctx);
    void OnRepair(InventoryContext _ctx);
}

public interface IEquipable
{
    void OnEquiped(InventoryContext _ctx);
}