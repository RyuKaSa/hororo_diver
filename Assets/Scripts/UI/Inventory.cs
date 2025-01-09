using UnityEngine;

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

    private InventoryData data;

    private InventoryContext context;

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
        // int _slotCount = display.Initialize(this);

        data = new InventoryData(12);
        initInventory();

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

    public Item AddItem(Item _item)
    {
        if (!data.SlotAvailable(_item)) return _item;

        data.AddItem(ref _item);

        if (display != null)
        {
            display.UpdateDisplay(data.items);
        }

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
        weapon.OnEquiped(context);

    }

    public Item[] Data => data.items;
    public InventoryContext Context => context;
}
