using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Ores")]
    public OreData manganeseOre;
    public OreData copperOre;
    public OreData cobaltOre;
    public OreData ironOre;

    [Header("Weapons")]
    public WeaponData harpoonHandgun;
    public WeaponData whalingHarpoonCannon;
    public WeaponData knife;
    public WeaponData pickaxe;

    [Header("Artifacts")]
    public ItemData[] artifacts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public ItemData GetItemByName(string itemName)
    // {
    //     // Search through all defined items
    //     ItemData[] allItems = new ItemData[]
    //     {
    //     manganeseOre, copperOre, cobaltOre, ironOre,
    //     harpoonHandgun, whalingHarpoonCannon, knife, pickaxe
    //     };

    //     // Concatenate artifacts to the array
    //     allItems = allItems.Concat(artifacts).ToArray();

    //     return allItems.FirstOrDefault(item => item != null && item.itemName == itemName);
    // }
}