using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which represents data of an ore.
/// </summary>
[CreateAssetMenu(menuName = "Items/Ore Data")]
public class OreData : ItemData
{
    public enum OrePurpose
    {
        Armor,
        Ammunition
    }

    public OrePurpose purpose;

    public static Item FromOreName(string oreName)
    {
        var itemDB = Utils.GetComponentFromGameObjectTag<ItemDatabase>("ItemDatabase");

        if (itemDB == null)
        {
            Debug.Log("Error: Could not find ItemDatabase component");
        }

        var oreData = itemDB.GetItemByName(oreName);
        if (oreData == null)
        {
            Debug.Log("Error: Could not find ore which called " + oreName);
        }
        return new Item(oreData, 1);

    }


}
