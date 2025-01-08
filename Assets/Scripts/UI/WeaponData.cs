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
    /// This method returns the GameObject which correspond to this weapon data.
    /// </summary>
    /// <returns></returns>
    public GameObject GetWeapon()
    {
        var go = GameObject.FindGameObjectWithTag(this.itemName);
        if (go == null)
        {
            Debug.Log("Error: Could not find GameObject '" + this.itemName + "'");
        }
        return go;
    }

}
