using UnityEngine;


/// <summary>
/// Class which represents a long range weapon in the game.
/// This class manage the animation of a long range weapon
/// and the attack process.
/// </summary>
public sealed class LongRangeWeapon : MonoBehaviour, IWeapons
{
    public Animator animator;

    [SerializeField] private Sprite icon;

    [SerializeField] private ItemData.ItemType itemType;
    [SerializeField] private int stackMaxCount = 1;

    public GameObject projectilePrefab;

    [SerializeField]
    private GameObject firePoint;

    [SerializeField]
    private GameObject firePointFlipped;


    [SerializeField]
    private float attackRange = 0.5f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private float attack;

    [SerializeField]
    private float projectileSpeed = 1f;


    [SerializeField]
    private string weaponName;

    [SerializeField]
    private float timeBetween2Attack;


    public string WeaponName()
    {
        return weaponName;
    }

    public void AttackProcessing(float attackBonus)
    {
        Debug.Log("Player attack with " + transform.name);

        var player = GameObject.FindGameObjectWithTag("Player");
        var direction = player.transform.position - transform.position;
        direction.z = 0;


        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer.flipX)
        {
            var projectileGameObject = Instantiate(projectilePrefab, firePointFlipped.transform.position, transform.rotation);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.Initialize(-projectileSpeed, attack + (attack * attackBonus), false);

            var projSprite = projectile.GetComponent<SpriteRenderer>();
            projSprite.flipX = true;

        }
        else
        {
            var projectileGameObject = Instantiate(projectilePrefab, firePoint.transform.position, transform.rotation);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.Initialize(projectileSpeed, attack + (attack * attackBonus), false);
        }
    }

    public bool WeaponAnimationIsPlaying()
    {
        if (animator == null)
        {
            return false;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if "Mining" is in progress
        return stateInfo.IsName("MiningAnimation");
    }

    public void OnEquiped(InventoryContext _ctx)
    {
        // var equippedWeapon = _ctx.EquippedWeapon;
        // equippedWeapon = this;
        _ctx.EquipWeapon(this);
        Debug.Log("Info: equip " + transform.name);
    }

    public void ApplyUpgrade(string attribute, float percentage)
    {
        switch (attribute.ToLower())
        {
            case "attack":
                attack *= (1 + percentage / 100f);
                Debug.Log($"LongRangeWeapon damage upgraded to: {attack}");
                break;
            case "range":
                attackRange *= (1 + percentage / 100f);
                Debug.Log($"LongRangeWeapon range upgraded to: {attackRange}");
                break;
        }
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
        return timeBetween2Attack;
    }

}