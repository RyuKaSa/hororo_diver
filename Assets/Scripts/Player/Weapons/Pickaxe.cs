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

    public string WeaponName()
    {
        return weaponName;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check collision with Ore Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            var ore = collision.gameObject.GetComponent<Ore>();

            if (ore != null)
            {
                Debug.Log("Hit ore");
                var pickOre = ore.HitOre();
                if (pickOre)
                {
                    AddOre(ore.Name());
                }
            }

        }
    }

    private void AddOre(string name)
    {
        var item = OreData.FromOreName(name);
        var inventory = Utils.GetComponentFromGameObjectTag<Inventory>("Player");
        inventory.AddItem(item);

    }

    public void AttackProcessing()
    {
        Debug.Log("Tigger mining anim");
        animator.SetBool("isMining", true);

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