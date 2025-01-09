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

    /// <summary>
    /// This method returns IWeapon class this weapon data.
    /// </summary>
    /// <returns></returns>
    public IWeapons GetWeapon()
    {
        var go = GameObject.FindGameObjectWithTag(this.itemName);
        if (go == null)
        {
            Debug.Log("Error: Could not find GameObject '" + this.itemName + "'");
        }

        var component = go.GetComponent<IWeapons>();
        if (component == null)
        {
            Debug.Log("Error: Could not find component");
        }
        return component;
    }

}
