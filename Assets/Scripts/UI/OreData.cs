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


}
