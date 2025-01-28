using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class Inventory : MonoBehaviour
{
    enum WeaponID
    {
        PICKAXE,
        KNIFE,
        HARPOON,
        CANON
    };

    [SerializeField] private InventoryDisplay display;
    [SerializeField] private AmmunitionData ammoData;

    [SerializeField] private int nbSlots = 13; // Nb slot in inventory (weapons slots include)

    private InventoryData data;
    private InventoryContext context;
    private AmmunitionCrafter crafter;

    private int ammoId;
    private Dictionary<string, Upgrade> upgradeMap;

    /// <summary>
    /// This method initialize Player's inventory with
    /// weapon (pickaxe, knife, harpoon)
    /// </summary>
    private void initInventory()
    {
        var itemDB = Utils.GetComponentFromGameObjectTag<ItemDatabase>("ItemDatabase");

        if (itemDB == null)
        {
            Debug.Log("Error: Could not find ItemDatabase component");
        }

        // Add base weapons to inventory
        Debug.Log("Init inve ICI");
        AddItem(new Item(itemDB.pickaxe, 1));
        AddItem(new Item(itemDB.knife, 1));
        AddItem(new Item(itemDB.harpoonHandgun, 1));
        AddItem(new Item(itemDB.ammoData, 15));
        AddItem(new Item(itemDB.ironOre, 15));
        AddItem(new Item(itemDB.cobaltOre, 15));


    }

    private void Awake()
    {
        Debug.Log("Awake called on " + gameObject.name);
        if (display == null)
        {
            Debug.Log("Display == null");
        }

        upgradeMap = GenerateRandomUpgrades();
        data = new InventoryData(nbSlots);
        display.Initialize(this);

        initInventory();

        crafter = new AmmunitionCrafter(data.items);

        if (display != null)
        {
            display.UpdateDisplay(data.items);
            // display.DisplayUpgrades(upgradeMap.Values.ToList(), this);
        }

        var pickaxeGameObject = GameObject.FindGameObjectWithTag("Pickaxe");
        if (pickaxeGameObject == null)
        {
            Debug.Log("Pickaxe gameObject not found");
        }
        context = new InventoryContext(0, pickaxeGameObject.GetComponent<Pickaxe>());
        Debug.Log("Creates context");
    }

    // M�thodes pour les boutons UI
    public void CraftSingleAmmo()
    {
        if (crafter.CanCraftAmmunition(ammoData, 0))
        {
            if (crafter.CraftAmmunition(ammoData, 0))
            {
                display?.UpdateDisplay(data.items);
            }
        }
        else
        {
            Debug.Log("Pas assez de ressources pour crafter une munition!");
        }
    }

    public void CraftFiveAmmo()
    {
        if (crafter.CanCraftAmmunition(ammoData, 1))
        {
            if (crafter.CraftAmmunition(ammoData, 1))
            {
                display?.UpdateDisplay(data.items);
            }
        }
        else
        {
            Debug.Log("Pas assez de ressources pour crafter 5 munitions!");
        }
    }

    public void CraftTwentyTenAmmo()
    {
        if (crafter.CanCraftAmmunition(ammoData, 2))
        {
            if (crafter.CraftAmmunition(ammoData, 2))
            {
                display?.UpdateDisplay(data.items);
            }
        }
        else
        {
            Debug.Log("Pas assez de ressources pour crafter 25 munitions!");
        }
    }

    public Dictionary<string, Upgrade> GenerateRandomUpgrades()
    {
        string[] attributes = { "damage", "resistance", "speed", "miningSpeed" };
        Dictionary<string, Upgrade> upgrades = new Dictionary<string, Upgrade>();

        for (int i = 0; i < attributes.Length; i++)
        {
            Dictionary<string, int> requiredOres = new Dictionary<string, int>
        {
            { "Iron", Random.Range(1, 5) },
            { "Manganese", Random.Range(0, 3) }
        };

            upgrades.Add(attributes[i], new Upgrade(attributes[i], 15f, requiredOres));
        }

        return upgrades;
    }

    public void GenerateRandomUpgradeForKey(string key)
    {
        string[] attributes = { "damage", "resistance", "speed", "miningSpeed" };
        Dictionary<string, int> requiredOres = new Dictionary<string, int>
    {
        { "Iron", Random.Range(1, 5) },
        { "Manganese", Random.Range(0, 3) }
    };

        if (attributes.Contains(key))
        {
            upgradeMap[key] = new Upgrade(key, 15f, requiredOres);
        }
    }

    public void ModifyItemQuantity(string itemName, int amount)
    {
        for (int i = 0; i < data.items.Length; i++)
        {
            if (data.items[i].Name == itemName)
            {
                data.items[i] = data.items[i].ModifyCount(amount);
                if (display != null)
                {
                    display.UpdateDisplay(data.items);
                }
                break;
            }
        }
    }

    public void OnInventoryOpen()
    {
        if (display != null)
        {
            display.DisplayUpgrades(upgradeMap.Values.ToList(), this);
        }
    }

    public void OnWeaponUpgradeOpen()
    {
    }

    public Item AddItem(Item _item)
    {
        Debug.Log("Info inventory: Add Item = " + _item.Data.itemType + " name = " + _item.Data.itemName);
        if (!data.SlotAvailable(_item)) return _item;

        data.AddItem(ref _item);

        if (display != null)
        {
            display.UpdateDisplay(data.items);
        }

        Debug.Log("Add Item " + _item);

        return _item;
    }

    private void AddWeapon(Item _item)
    {
        data.AddItem(ref _item);
        display.UpdateDisplay(data.items);

    }

    public Item PickItem(int _slotID)
    {
        Item _result = data.Pick(_slotID);

        display.UpdateDisplay(data.items);

        return _result;
    }

    public void SwapSlot(int _slotA, int _slotB)
    {
        data.Swap(_slotA, _slotB);

        display.UpdateDisplay(data.items);
    }

    public void SwapWeapon(int acc)
    {
        int id = acc % 3;

        Item item = data.Peek(id);
        var weaponData = item.Data as WeaponData;
        if (weaponData == null)
        {
            Debug.Log("Error: weapon data is null");
        }
        var weapon = weaponData.GetWeapon();
        context.EquipWeapon(weapon);

    }

    public int GetAmmo()
    {
        foreach (var item in data.items)
        {
            if (item.Data.itemName == "harpoonAmmo")
            {
                Debug.Log(" item.Data.itemName = " + item.Data.itemName + " item.Count = " + item.Count);
                return item.Count;
            }
        }
        return 0;
    }

    public int UpdateAmmo()
    {
        for (int i = 0; i < data.items.Length; i++)
        {
            if (data.items[i].Data.itemName == "harpoonAmmo")
            {
                if (data.items[i].count > 0)
                {
                    data.items[i].count -= 1;
                }
            }
        }
        return 0;
    }


    // Getter for upgradeMap
    public Dictionary<string, Upgrade> GetUpgradeMap()
    {
        return upgradeMap;
    }

    public Item[] Data => data.items;
    public InventoryContext Context => context;
}
