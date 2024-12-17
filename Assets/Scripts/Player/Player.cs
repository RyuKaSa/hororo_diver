using UnityEngine;


public sealed class Player : MonoBehaviour, IDamageable
{


    [SerializeField]
    private Player_Input playerInput;

    [SerializeField]
    private float health = 15f;

    [SerializeField]
    private GameObject[] weapons; // Array of 2 elements which is 2 slots of Player's weapons

    private int weaponId = 0;

    [SerializeField]
    private float weaponSwappingTime = 0.5f;

    private IWeapons currentWeapon;

    private bool isSwapping = false; // Boolean which indicates Player is changing the weapon that hold

    private float currentTimeSwap = 0f;

    public void Start()
    {
        currentWeapon = weapons[weaponId].GetComponent<IWeapons>();
        if (currentWeapon == null)
        {
            Debug.Log("Weapon not found");
        }
    }

    public void Update()
    {
        playerInput.Update(); // Updates movement
        var action = playerInput.GetPlayerActionByKey();
        SwapWeapon();

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
            currentWeapon = weapons[weaponId % 2].GetComponent<IWeapons>();
        }
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }


}