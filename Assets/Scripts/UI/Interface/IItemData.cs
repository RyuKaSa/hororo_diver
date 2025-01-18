using UnityEngine;

/// <summary>
/// Interface which representes basic propoerties of an Item
/// </summary>
public interface IItemData
{
    // public enum ItemType
    // {
    //     Ore_Armor,
    //     Ore_Ammunition,
    //     Weapon,
    //     Tool,
    //     Artifact
    // }

    public string ItemName();

    public ItemData.ItemType ItemType();

    public Sprite Icon();

    public int StackMaxCount();
}