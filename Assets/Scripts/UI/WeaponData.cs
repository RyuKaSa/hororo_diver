using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class WeaponData : ItemData
{
    public enum WeaponType
    {
        Harpoon,
        Cannon,
        Knife,
        Pickaxe
    }

    public WeaponType weaponType;
    public bool isStartingWeapon;
}
