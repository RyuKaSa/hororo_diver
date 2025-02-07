using UnityEngine;
using static ItemData;



public sealed class Pickaxe : MonoBehaviour, IWeapons
{

    [SerializeField] public Sprite icon;

    [SerializeField] private ItemData.ItemType itemType;
    [SerializeField] private int stackMaxCount = 1;

    public BoxCollider2D boxCollider;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string weaponName;

    [SerializeField]
    private LayerMask enemyLayer;

    private float attackRange = 1.5f;

    private float attack = 0.15f;

    private bool isHitting = false; // Flag to prevent multiple hits during the same animation

    public string WeaponName()
    {
        return weaponName;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check collision with Ore Layer
        if (!isHitting && collision.gameObject.layer == LayerMask.NameToLayer("Ore") && (animator.GetBool("isMining") || animator.GetBool("isMiningRev")))
        {
            var ore = collision.gameObject.GetComponent<Ore>();

            if (ore != null)
            {
                isHitting = true;
                Debug.Log("Hit ore gameObject name = " + gameObject.name);
                var pickOre = ore.HitOre();
                if (pickOre)
                {
                    AddOre(ore.Name());
                }
            }

        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // Reset hitting state when exiting collision
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            isHitting = false;
        }
    }

    private void AddOre(string name)
    {
        var item = OreData.FromOreName(name);
        var inventory = GameObject.Find("Inventory UI").GetComponent<Inventory>();
        inventory.AddItem(item);

    }

    public void AttackProcessing(float attackBonus)
    {
        if (GetComponent<SpriteRenderer>().flipX)
        {
            animator.SetBool("isMiningRev", true);
        }
        else
        {
            animator.SetBool("isMining", true);
        }

        Debug.Log("Player use Pickaxe ");

        var hitEnnemiesArray = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (var collider in hitEnnemiesArray)
        {
            var damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log("Interface was found");
                damageable.Damage(attack);
            }
        }
    }

    public void ResetAnimationFlag()
    {
        animator.SetBool("isMining", false);
        animator.SetBool("isMiningRev", false);

        Debug.Log("Mining animation end");
    }

    public void ApplyUpgrade(string attribute, float percentage)
    {
        switch (attribute.ToLower())
        {
            case "attack":
                attack *= (1 + percentage / 100f);
                Debug.Log($"Pickaxe damage upgraded to: {attack}");
                break;
            case "range":
                attackRange *= (1 + percentage / 100f);
                Debug.Log($"Pickaxe range upgraded to: {attackRange}");
                break;
        }
    }

    public bool WeaponAnimationIsPlaying()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("getBool = " + animator.GetBool("isMining"));

        // Check if "Mining" is in progress
        return animator.GetBool("isMining");
    }

    public void OnEquiped(InventoryContext _ctx)
    {
        // var equippedWeapon = _ctx.EquippedWeapon;
        // equippedWeapon = this;
        _ctx.EquipWeapon(this);
        Debug.Log("Info: equip " + transform.name);

    }

    public Sprite Icon()
    {
        return icon;
    }

    public ItemData.ItemType ItemType()
    {
        return itemType;
    }

    public int StackMaxCount()
    {
        return stackMaxCount;
    }

    public float TimeBetween2Attack()
    {
        return 0.5f;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }


}