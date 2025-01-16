using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private int draggedSlotIndex;

    private StatsDisplay statsDisplay;

    [SerializeField] private InventoryContextMenu contextMenu;
    private Slot[] slots;
    private Inventory inventory;

    [SerializeField] private Button[] upgradeButtons;

    public int Initialize(Inventory _inventory)
    {
        slots = GetComponentsInChildren<Slot>();
        inventory = _inventory;

        statsDisplay = FindObjectOfType<StatsDisplay>();

        contextMenu.Init(_inventory);

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Initialize(this, i);
        }

        return slots.Length;
    }

    public void UpdateDisplay(Item[] _items)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateDisplay(_items[i]);
        }
    }

    public void DisplayUpgrades(List<Upgrade> upgrades, Inventory inventory)
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            Button button = upgradeButtons[i];

            if (i >= upgrades.Count)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            Upgrade upgrade = upgrades[i];

            string requirements = string.Join("\n", upgrade.RequiredOres.Select(r => $"{r.Key}: {r.Value}"));
            button.GetComponentInChildren<Text>().text =
                $"Amélioration: {upgrade.Attribute}\n" +
                $"+{upgrade.Percentage:F1}%\n" +
                $"Coût:\n{requirements}";

            button.onClick.RemoveAllListeners();
            int upgradeIndex = i; // Capture l'index pour le listener
            button.onClick.AddListener(() =>
            {
                if (ApplyUpgrade(upgrades[upgradeIndex], inventory))
                {
                    Debug.Log($"{upgrade.Attribute} amélioré de {upgrade.Percentage}% !");
                    button.interactable = false;
                }
                else
                {
                    Debug.Log("Ressources insuffisantes !");
                }
            });
        }
    }

    public bool ApplyUpgrade(Upgrade upgrade, Inventory inventory)
    {
        // Vérification des ressources
        foreach (var ore in upgrade.RequiredOres)
        {
            bool hasEnoughResources = false;
            foreach (var item in inventory.Data)
            {
                if (item.Name == ore.Key && item.Quantity >= ore.Value)
                {
                    hasEnoughResources = true;
                    break;
                }
            }
            if (!hasEnoughResources)
            {
                Debug.Log($"Pas assez de {ore.Key} : requis {ore.Value}");
                return false;
            }
        }

        // Consommation des ressources
        foreach (var ore in upgrade.RequiredOres)
        {
            inventory.ModifyItemQuantity(ore.Key, -ore.Value);
        }

        // Application de l'amélioration
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            // Le constructeur de StatModifier attend un int, donc on convertit le pourcentage
            StatModifier modifier = new StatModifier(
                (int)upgrade.Percentage,
                1, // ordre positif pour un bonus
                StatModifier.StatModifierType.PERCENTAGE
            );

            if (player.AddStatModifierToAttribute(upgrade.Attribute, modifier))
            {
                Debug.Log($"Attribut {upgrade.Attribute} amélioré de {(int)upgrade.Percentage}%");

                // Mettre à jour l'affichage des stats
                if (statsDisplay != null)
                {
                    statsDisplay.UpdateStats();
                }

                return true;
            }
        }

        Debug.Log("Player non trouvé");
        return false;
    }

    public List<WeaponUpgrade> GenerateRandomWeaponUpgrades()
    {
        Dictionary<string, string[]> weaponAttributes = new Dictionary<string, string[]>
    {
        { "Pickaxe", new[] { "attack", "range" } },
        { "MeleeWeapon", new[] { "attack", "range" } },
        { "LongRangeWeapon", new[] { "attack", "range" } }
    };

        List<WeaponUpgrade> upgrades = new List<WeaponUpgrade>();

        // Générer 3 améliorations aléatoires
        for (int i = 0; i < 3; i++)
        {
            // Choisir une arme aléatoire
            string weaponType = weaponAttributes.Keys.ElementAt(Random.Range(0, weaponAttributes.Count));
            string[] possibleAttributes = weaponAttributes[weaponType];
            string randomAttribute = possibleAttributes[Random.Range(0, possibleAttributes.Length)];
            float randomPercentage = Random.Range(5f, 20f);

            Dictionary<string, int> requiredOres = new Dictionary<string, int>
        {
            { "IronOre", Random.Range(2, 6) },
            { "GoldOre", Random.Range(1, 4) }
        };

            upgrades.Add(new WeaponUpgrade(weaponType, randomAttribute, randomPercentage, requiredOres));
        }

        return upgrades;
    }

    public bool ApplyWeaponUpgrade(WeaponUpgrade upgrade, Inventory inventory)
    {
        // Vérification des ressources
        foreach (var ore in upgrade.RequiredOres)
        {
            bool hasEnoughResources = false;
            foreach (var item in inventory.Data)
            {
                if (item.Name == ore.Key && item.Quantity >= ore.Value)
                {
                    hasEnoughResources = true;
                    break;
                }
            }
            if (!hasEnoughResources)
            {
                Debug.Log($"Pas assez de {ore.Key} : requis {ore.Value}");
                return false;
            }
        }

        // Rechercher l'arme dans les enfants du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            IWeapons weapon = null;

            // Trouver le bon composant d'arme selon le type
            switch (upgrade.WeaponType)
            {
                case "Pickaxe":
                    weapon = player.GetComponentInChildren<Pickaxe>();
                    break;
                case "MeleeWeapon":
                    weapon = player.GetComponentInChildren<MeleeWeapon>();
                    break;
                case "LongRangeWeapon":
                    weapon = player.GetComponentInChildren<LongRangeWeapon>();
                    break;
            }

            if (weapon == null)
            {
                Debug.Log($"Arme de type {upgrade.WeaponType} non trouvée");
                return false;
            }

            // Consommer les ressources
            foreach (var ore in upgrade.RequiredOres)
            {
                inventory.ModifyItemQuantity(ore.Key, -ore.Value);
            }

            // Appliquer l'amélioration
            weapon.ApplyUpgrade(upgrade.Attribute, upgrade.Percentage);
            Debug.Log($"{upgrade.WeaponType} amélioré : {upgrade.Attribute} +{upgrade.Percentage}%");

            return true;
        }

        Debug.Log("Player non trouvé");
        return false;
    }

    public void DisplayWeaponUpgrades(List<WeaponUpgrade> upgrades, Inventory inventory)
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            Button button = upgradeButtons[i];

            if (i >= upgrades.Count)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            WeaponUpgrade upgrade = upgrades[i];

            // Traduire les attributs en français
            string attributeFr = upgrade.Attribute.ToLower() == "attack" ? "Dégâts" : "Portée";

            string requirements = string.Join("\n", upgrade.RequiredOres.Select(r => $"{r.Key}: {r.Value}"));
            button.GetComponentInChildren<Text>().text =
                $"Arme: {upgrade.WeaponType}\n" +
                $"{attributeFr} +{upgrade.Percentage:F1}%\n" +
                $"Coût:\n{requirements}";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (ApplyWeaponUpgrade(upgrade, inventory))
                {
                    button.interactable = false;
                }
            });
        }
    }

    #region Inputs
    public void ClickSlot(int _index)
    {
        contextMenu.Select(_index, slots[_index]);
    }

    public void DragSlot(int _index) => draggedSlotIndex = _index;

    public void DropOnSlot(int _index)
    {
        inventory.SwapSlot(draggedSlotIndex, _index);
    }

    #endregion
}
