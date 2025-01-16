using UnityEngine;



public sealed class Pickaxe : MonoBehaviour, IWeapons
{

    public Sprite icon;

    public BoxCollider2D boxCollider;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string weaponName;

    [SerializeField]
    private ItemData.ItemType weaponType;

    [SerializeField]
    private int stackMaxCount;

    [SerializeField]
    private LayerMask enemyLayer;

    private float attackRange = 1.5f;

    private float attack = 0.15f;

    public string ItemName()
    {
        return weaponName;
    }

    public Sprite Icon()
    {
        return icon;
    }

    public ItemData.ItemType ItemType()
    {
        return weaponType;
    }

    public int StackMaxCount()
    {
        return stackMaxCount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check collision with Ore Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            Debug.Log("Pickaxe collided with Ore!");
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
        TriggerAnimation();
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

    public void TriggerAnimation()
    {
        Debug.Log("Tigger mining anim");
        animator.SetBool("isMining", true);
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

}