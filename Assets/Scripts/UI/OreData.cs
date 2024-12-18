using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
