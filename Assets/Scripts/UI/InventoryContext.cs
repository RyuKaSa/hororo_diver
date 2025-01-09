using System;

[System.Serializable]
public struct InventoryContext
{
    private int currentItemCount;
    private int maxItemCount;
    private bool isInventoryFull;
    private DateTime lastUpdated;
    private IWeapons equippedWeapon;

    public InventoryContext(int maxItemCount, IWeapons equippedWeapon = null)
    {
        this.currentItemCount = 0;
        this.maxItemCount = maxItemCount;
        this.isInventoryFull = false;
        this.lastUpdated = DateTime.Now;
        this.equippedWeapon = equippedWeapon;
    }

    public IWeapons EquippedWeapon => equippedWeapon;

    public void AddItem()
    {
        if (currentItemCount < maxItemCount)
        {
            currentItemCount++;
            lastUpdated = DateTime.Now;
            if (currentItemCount == maxItemCount)
            {
                isInventoryFull = true;
            }
        }
        else
        {
            Console.WriteLine("L'inventaire est plein.");
        }
    }

    public void RemoveItem()
    {
        if (currentItemCount > 0)
        {
            currentItemCount--;
            isInventoryFull = false;
            lastUpdated = DateTime.Now;
        }
        else
        {
            Console.WriteLine("L'inventaire est vide.");
        }
    }

    public void EquipWeapon(IWeapons weapon)
    {
        equippedWeapon = weapon;
        //Console.WriteLine($"L'arme équipée est : {equippedWeapon.GetWeaponName()}");
    }

    public void UnequipWeapon()
    {
        equippedWeapon = null;
        Console.WriteLine("Aucune arme équipée.");
    }

    public void DisplayStatus()
    {
        Console.WriteLine($"Objets dans l'inventaire : {currentItemCount}/{maxItemCount}");
        Console.WriteLine($"Inventaire plein : {isInventoryFull}");
        Console.WriteLine($"Dernière mise à jour : {lastUpdated}");

    }
}
