using UnityEngine;


public sealed class Player : MonoBehaviour, IDamageable
{


    [SerializeField]
    private Player_Input playerInput;

    [SerializeField]
    private float health = 15f;

    [SerializeField]
    private GameObject meleeWeapon;

    [SerializeField]
    private GameObject longRangeWeapon;

    [SerializeField]
    private float weaponSwappingTime = 0.5f;

    private IWeapons currentWeapon;

    private bool isSwapping = false; // Boolean which indicates Player is changing the weapon that hold

    private float currentTimeSwap = 0f;

    public void Start()
    {
        currentWeapon = meleeWeapon.GetComponent<IWeapons>();
        currentWeapon = longRangeWeapon.GetComponent<IWeapons>();
        if (currentWeapon == null)
        {
            Debug.Log("Weapon melee not found");
        }
    }

    public void Update()
    {
        playerInput.Update();
        if (health <= 0)
        {
            Debug.Log("Player is dead");
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentWeapon.AttackProcessing();
        }
    }

    private void SwapWeapon()
    {
        if (!isSwapping)
        {
            isSwapping = true;
        }

        if (isSwapping)
        {
            currentTimeSwap += Time.deltaTime;
        }
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }


}