using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;


public sealed class Player : MonoBehaviour, IDamageable
{

    [SerializeField]
    private Player_Input playerInput;

    [SerializeField]
    private float health = 15f, damage, miningSpeed, resistance, speed;

    [SerializeField]
    private GameObject[] weapons; // Array of 3 elements which is 3 slots of Player's weapons (Pickaxe include)

    [SerializeField]
    private GameObject pickaxe;

    [SerializeField]
    private Inventory inventory;

    private int weaponId = 0;

    [SerializeField]
    private float weaponSwappingTime = 0.5f;

    private IWeapons currentWeapon;

    private bool isSwapping = false; // Boolean which indicates Player is changing the weapon that hold

    private float currentTimeSwap = 0f;

    // Il faudrait une liste non modifiable des noms d'attributs existant accessible par toutes les classes (penser à utiliser AsReadonly class List) 
    private readonly Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

    private ReadOnlyDictionary<string, Attribute> attributesReadOnly;

    private int debugCmpt = 0;

    public void Start()
    {

        DisabledWeaponNotHolding();

        // Init Attribute map based on field
        attributes.Add("damage", new Attribute(damage));
        attributes.Add("miningSpeed", new Attribute(miningSpeed));
        attributes.Add("resistance", new Attribute(resistance));
        attributes.Add("speed", new Attribute(speed));

        attributesReadOnly = new ReadOnlyDictionary<string, Attribute>(attributes);
    }

    private void Awake()
    {
        Debug.Log("Creates Player");
        currentWeapon = inventory.Context.EquippedWeapon;
        if (currentWeapon == null)
        {
            Debug.Log("Weapon not found");
        }

    }

    private void DebugAttributeTest()
    {
        if (debugCmpt == 0)
        {
            attributes["speed"].AddStatModifier(new StatModifier(5, 1, StatModifier.StatModifierType.ADDITIONAL));
            debugCmpt++;
        }
    }

    public void Update()
    {
        // Weapon follow Player's hand
        var hand = transform.Find("HandPoint");
        weapons[weaponId].transform.position = hand.transform.position;

        playerInput.Update(); // Updates movement
        SwapWeapon();

        var action = playerInput.GetPlayerActionByKey();
        if (health <= 0)
        {
            Debug.Log("Player is dead");
        }

        if (action == Player_Input.INPUT_ACTION.ATTACK_ACTION && !isSwapping)
        {
            Debug.Log("Player attack");
            currentWeapon.AttackProcessing();

        }

        if (action == Player_Input.INPUT_ACTION.SWAP_WEAPON_ACTION! && !isSwapping)
        {
            Debug.Log("Player swap weapon");
            weaponId += 1;
            isSwapping = true;
        }
    }

    private void SwapWeapon()
    {
        if (isSwapping)
        {
            Debug.Log("Current swapping");
            currentTimeSwap += Time.deltaTime;
        }

        if (currentTimeSwap >= weaponSwappingTime)
        {
            Debug.Log("Finish to swap");
            isSwapping = false;
            currentTimeSwap = 0f;
            currentWeapon = weapons[weaponId % 2].GetComponent<IWeapons>();
            var currentWeaponRender = weapons[weaponId % 2].GetComponent<Renderer>();
            currentWeaponRender.enabled = true;

            DisabledWeaponNotHolding(); // Disabled other Renderer

        }
    }

    private void DisabledWeaponNotHolding()
    {
        var otherWeapon = weapons[(weaponId + 1) % 2].GetComponent<Renderer>();
        if (otherWeapon == null)
        {
            Debug.Log("Renderer for " + weapons[(weaponId + 1) % 2].transform.name + " not found");
            return;
        }
        otherWeapon.enabled = false;
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }

    public bool AddStatModifierToAttribute(string attribute, StatModifier statModifier)
    {
        if (attribute == null || statModifier == null)
        {
            Debug.Log("Attribute or statModifier is null");
            return false;
        }

        if (!attributes.ContainsKey(attribute))
        {
            Debug.Log("Attribute in parameter doesn't exists");
            return false;

        }

        // Add statModifier to the attribute
        attributes[attribute].AddStatModifier(statModifier);
        return true;
    }

    public ReadOnlyDictionary<string, Attribute> AsReadOnlyAttributes()
    {
        return attributesReadOnly;
    }

}