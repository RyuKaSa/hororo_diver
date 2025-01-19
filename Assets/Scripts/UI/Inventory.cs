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

    private InventoryData data;
    private InventoryContext context;
    private AmmunitionCrafter crafter;

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
        AddItem(new Item(itemDB.pickaxe, 1));
        AddItem(new Item(itemDB.knife, 1));
        AddItem(new Item(itemDB.harpoonHandgun, 1));

    }

    private void Awake()
    {
        data = new InventoryData(12);
        initInventory();

        crafter = new AmmunitionCrafter(data.items);

        if (display != null)
        {
            display.UpdateDisplay(data.items);
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

    public List<Upgrade> GenerateRandomUpgrades()
    {
        string[] attributes = { "damage", "miningSpeed", "resistance", "speed" };
        List<Upgrade> upgrades = new List<Upgrade>();

        for (int i = 0; i < 3; i++)
        {
            string randomAttribute = attributes[Random.Range(0, attributes.Length)];
            float randomPercentage = Random.Range(5f, 20f); // 5% � 20% d'am�lioration.
            Dictionary<string, int> requiredOres = new Dictionary<string, int>
        {
            { "IronOre", Random.Range(1, 5) },
            { "GoldOre", Random.Range(0, 3) }
        };

            upgrades.Add(new Upgrade(randomAttribute, randomPercentage, requiredOres));
        }

        return upgrades;
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
        List<Upgrade> upgrades = GenerateRandomUpgrades();
        if (display != null)
        {
            display.DisplayUpgrades(upgrades, this);
        }
    }

    public void OnWeaponUpgradeOpen()
    {
        // Utiliser la méthode de l'InventoryDisplay pour générer les améliorations
        List<WeaponUpgrade> weaponUpgrades = display.GenerateRandomWeaponUpgrades();
        if (display != null)
        {
            display.DisplayWeaponUpgrades(weaponUpgrades, this);
        }
    }

    public Item AddItem(Item _item)
    {
        if (!data.SlotAvailable(_item)) return _item;

        data.AddItem(ref _item);

        if (display != null)
        {
            display.UpdateDisplay(data.items);
        }

        Debug.Log("Add Item " + _item);

        return _item;
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

    public Item[] Data => data.items;
    public InventoryContext Context => context;
}
