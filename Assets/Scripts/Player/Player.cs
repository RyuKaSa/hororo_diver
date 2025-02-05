using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Transform spawnPoint;

    public event System.Action OnStatsChanged;

    public event System.Action OnHealthChanged;

    [SerializeField]
    private Player_Input playerInput;

    [SerializeField]
    private float health = 15f, baseHealth = 15f, damage, miningSpeed, resistance, speed;

    [SerializeField]
    private int nbLife = 3;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private float weaponSwappingTime = 0.5f;

    [SerializeField]
    private ColoredFlash coloredFlash;

    private int weaponId = 0;

    private IWeapons currentWeapon;

    private bool isSwapping = false; // Boolean which indicates Player is changing the weapon that hold

    private float currentTimeSwap = 0f;

    private readonly Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

    private ReadOnlyDictionary<string, Attribute> attributesReadOnly;

    private StateMachine<PlayerStates> stateMachine = new StateMachine<PlayerStates>();

    private int debugCmpt = 0;

    private float oxygenAmount = 100f;

    public float drowningDamage = 2.3f;

    public float oxygenLossPerFrame = 15f;

    public float oxygenGainPerFrame = 30f;

    private bool attackTrigger = false;

    private float timeBetween2AttackInput = 0f;

    public float invincibilityDuration = 1f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;



    public void Start()
    {
        // Set state machine states
        stateMachine.AddState(PlayerStates.IDLE, new State<PlayerStates>(
            onLogic: state =>
            {
                playerInput.UpdateMovement();
                oxygenAmount += oxygenGainPerFrame * Time.deltaTime;

                // fix limit to 100
                if (oxygenAmount > 100f)
                {
                    oxygenAmount = 100f;
                }

                // Weapon follow Player's hand
                var isFlipped = GetComponent<SpriteRenderer>().flipX;
                var hand = isFlipped ? transform.Find("HandPointFlip") : transform.Find("HandPoint");

                var weaponGO = GameObject.FindGameObjectWithTag(currentWeapon.WeaponName());
                var spriteRenderer = weaponGO.GetComponent<SpriteRenderer>();

                spriteRenderer.flipX = isFlipped ? true : false;
                weaponGO.transform.position = hand.transform.position;


            }
        ));

        stateMachine.AddState(PlayerStates.MOVE, new State<PlayerStates>(
            onLogic: state =>
            {
                var weaponGO = GameObject.FindGameObjectWithTag(currentWeapon.WeaponName());
                weaponGO.transform.position = new Vector3(-1000f, -1000f, 0f);

                playerInput.UpdateMovement();
                oxygenAmount -= oxygenLossPerFrame * Time.deltaTime;
                if (oxygenAmount < 0f)
                {
                    oxygenAmount = 0f;
                }
            }
        ));

        stateMachine.AddState(PlayerStates.ATTACK, new State<PlayerStates>(
            onLogic: state =>
            {
                playerInput.UpdateMovement();
                if (currentWeapon.WeaponName() != "Harpoon")
                {
                    currentWeapon.AttackProcessing(attributes["damage"].FinalValue());
                }
                else if (inventory.GetAmmo() > 0)
                {
                    currentWeapon.AttackProcessing(attributes["damage"].FinalValue());
                    inventory.UpdateAmmo();
                }
            }
        ));

        stateMachine.AddState(PlayerStates.SWAP, new State<PlayerStates>(
            onLogic: state =>
            {
                playerInput.UpdateMovement(); // Keep rotation
                var go = GameObject.FindGameObjectWithTag(currentWeapon.WeaponName());

                go.transform.position = new Vector3(-1000, -1000, -1000);
                inventory.SwapWeapon(weaponId);
                currentWeapon = inventory.Context.GetEquippedWeapon();

                go = GameObject.FindGameObjectWithTag(currentWeapon.WeaponName());
                go.transform.position = transform.Find("HandPoint").position;

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
            transition =>
            {
                bool cond = playerInput.GetPlayerActionByKey() == Player_Input.INPUT_ACTION.ATTACK_ACTION;
                if (cond && timeBetween2AttackInput >= currentWeapon.TimeBetween2Attack())
                {
                    timeBetween2AttackInput = Time.deltaTime;
                    return true;
                }
                return false;
            }
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
        // if (isInvincible)
        // {
        //     invincibilityTimer -= Time.deltaTime;
        //     if (invincibilityTimer <= 0f)
        //     {
        //         isInvincible = false;
        //     }
        // }

        if (health <= 0)
        {
            nbLife -= 1;
            if (nbLife > 0)
            {
                transform.position = spawnPoint.position;
                // Debug.Log("Info in Player: player respawn to " + transform.position + " Safezone" + GameObject.Find("SafeZone").transform.position);
                health = baseHealth;
                oxygenAmount = 100f;

            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (oxygenAmount <= 0f)
        {
            health -= drowningDamage * Time.deltaTime;
        }

        timeBetween2AttackInput += Time.deltaTime;
        var action = playerInput.GetPlayerActionByKey();

        // Avoid multiple swapping by stay in SWAP state during weaponSwappingTime
        if (action == Player_Input.INPUT_ACTION.SWAP_WEAPON_ACTION)
        {
            Debug.Log("Player swap weapon");
            weaponId += 1;
        }

        stateMachine.OnLogic();
    }

    public void Damage(float damage)
    {

        // if (isInvincible)
        // {
        //     Debug.Log(transform.name + " is invincible and takes no damage.");
        //     return;
        // }

        Debug.Log(transform.name + " takes " + damage + " damage");
        health = (health + attributes["resistance"].FinalValue()) - damage;
        OnHealthChanged?.Invoke();

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
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

        // Notifier les changements
        OnStatsChanged?.Invoke();

        return true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("BasicMob") || other.gameObject.layer == LayerMask.NameToLayer("MisterFish"))
        {
            Debug.Log("Receive damage");
            coloredFlash.Flash(Color.red);
        }
    }


    public ReadOnlyDictionary<string, Attribute> AsReadOnlyAttributes()
    {
        return attributesReadOnly;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetOxygen()
    {
        return oxygenAmount;
    }

    public int GetLife()
    {
        return nbLife;
    }

}