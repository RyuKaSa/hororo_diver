using UnityEngine;

[CreateAssetMenu(menuName ="Items/Item Data")]
public class ItemData : ScriptableObject
{
    /*[SerializeField] public string itemName;

    [SerializeField] public int stackMaxCount = 1;

    [SerializeField] public Sprite icon;*/

    public enum ItemType
    {
        Ore_Armor,
        Ore_Ammunition,
        Weapon,
        Tool,
        Artifact
    }

    [Header("Basic Item Properties")]
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public int stackMaxCount = 1;

    [Header("Ore Specific")]
    public int miningLevel; // Which levels this ore can be found in
    public bool isRare;

    [Header("Weapon Specific")]
    public float baseDamage;
    public bool isCraftable;
    public string[] requiredOres; // Ores needed to craft this weapon

    [Header("Artifact Specific")]
    public bool isUnique;
    public bool isMandatory; // Unmissable artifact
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