using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public sealed class Player : MonoBehaviour, IDamageable
{
    enum PlayerStates
    {
        IDLE,
        MOVE,
        ATTACK,
        SWAP
    };


    [SerializeField]
    private Player_Input playerInput;

    [SerializeField]
    private float health = 15f, damage, miningSpeed, resistance, speed;

    [SerializeField]
    private GameObject[] weapons; // Array of 3 elements which is 3 slots of Player's weapons (Pickaxe include)

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private float weaponSwappingTime = 0.5f;

    private int weaponId = 0;

    private IWeapons currentWeapon;

    private bool isSwapping = false; // Boolean which indicates Player is changing the weapon that hold

    private float currentTimeSwap = 0f;

    // Il faudrait une liste non modifiable des noms d'attributs existant accessible par toutes les classes (penser à utiliser AsReadonly class List) 
    private readonly Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

    private ReadOnlyDictionary<string, Attribute> attributesReadOnly;

    private StateMachine<PlayerStates> stateMachine = new StateMachine<PlayerStates>();

    private int debugCmpt = 0;

    public void Start()
    {
        // Set state machine states
        stateMachine.AddState(PlayerStates.IDLE, new State<PlayerStates>(
            onLogic: state => { playerInput.UpdateMovement(); Debug.Log("IDLE STATE"); }
        ));

        stateMachine.AddState(PlayerStates.MOVE, new State<PlayerStates>(
            onLogic: state => playerInput.UpdateMovement()
        ));

        stateMachine.AddState(PlayerStates.ATTACK, new State<PlayerStates>(
            onLogic: state => currentWeapon.AttackProcessing()
        ));

        stateMachine.AddState(PlayerStates.SWAP, new State<PlayerStates>(
            onLogic: state =>
            {
                weaponId += 1;
                Debug.Log("Dans swap state");
                inventory.SwapWeapon(weaponId);
            },
            canExit: state => state.timer.Elapsed > weaponSwappingTime,
            needsExitTime: true
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.IDLE,
            PlayerStates.MOVE,
            transition => playerInput.MovementButtonIsTriggered()
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.MOVE,
            PlayerStates.IDLE,
            transition => !playerInput.MovementButtonIsTriggered()
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.MOVE,
            PlayerStates.MOVE,
            transition => playerInput.MovementButtonIsTriggered()
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.IDLE,
            PlayerStates.ATTACK,
            transition => playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.ATTACK_ACTION
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.ATTACK,
            PlayerStates.MOVE,
            transition => playerInput.MovementButtonIsTriggered() && !currentWeapon.WeaponAnimationIsPlaying()
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.ATTACK,
            PlayerStates.IDLE,
            transition => !currentWeapon.WeaponAnimationIsPlaying()
        ));


        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.MOVE,
            PlayerStates.ATTACK,
            transition => playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.ATTACK_ACTION
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.IDLE,
            PlayerStates.SWAP,
            transition => playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.SWAP_WEAPON_ACTION
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.SWAP,
            PlayerStates.IDLE,
            transition => true
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.SWAP,
            PlayerStates.ATTACK,
            transition => playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.ATTACK_ACTION
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.SWAP,
            PlayerStates.MOVE,
            transition => playerInput.MovementButtonIsTriggered()
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.MOVE,
            PlayerStates.SWAP,
            transition => playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.SWAP_WEAPON_ACTION
        ));

        stateMachine.AddTransition(new Transition<PlayerStates>(
            PlayerStates.ATTACK,
            PlayerStates.SWAP,
            transition => playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.SWAP_WEAPON_ACTION && !currentWeapon.WeaponAnimationIsPlaying()
        ));


        stateMachine.SetStartState(PlayerStates.IDLE);

        DisabledWeaponNotHolding();

        // Init Attribute map based on field
        attributes.Add("damage", new Attribute(damage));
        attributes.Add("miningSpeed", new Attribute(miningSpeed));
        attributes.Add("resistance", new Attribute(resistance));
        attributes.Add("speed", new Attribute(speed));

        attributesReadOnly = new ReadOnlyDictionary<string, Attribute>(attributes);

        stateMachine.Init();
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
        stateMachine.OnLogic();
        // Weapon follow Player's hand
        var hand = transform.Find("HandPoint");
        // weapons[weaponId].transform.position = hand.transform.position;

        var action = playerInput.GetPlayerActionByKey();
        if (health <= 0)
        {
            Debug.Log("Player is dead");
        }

        if (action == Player_Input.INPUT_ACTION.SWAP_WEAPON_ACTION! && !isSwapping)
        {
            Debug.Log("Player swap weapon");
            weaponId += 1;
            isSwapping = true;
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