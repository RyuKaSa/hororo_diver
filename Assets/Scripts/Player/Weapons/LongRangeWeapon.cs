using UnityEngine;


/// <summary>
/// Class which represents a long range weapon in the game.
/// This class manage the animation of a long range weapon
/// and the attack process.
/// </summary>
public sealed class LongRangeWeapon : MonoBehaviour, IWeapons
{
    public Animator animator;

    public GameObject projectilePrefab;

    [SerializeField]
    private GameObject firePoint;

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


    public string WeaponName()
    {
        return weaponName;
    }


    public void AttackProcessing()
    {
        Debug.Log("Player attack with " + transform.name);

        var player = GameObject.Find("Player");
        var direction = player.transform.position - transform.position;
        direction.z = 0;
        float angle = Mathf.Atan2(direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;

        // Calculate Linear equation with mob position and player position
        float gradient = (player.transform.position.y - transform.position.y) / (player.transform.position.x - transform.position.x);
        float offset = transform.position.y - (gradient * transform.position.x);

        var projectileGameObject = Instantiate(projectilePrefab, firePoint.transform.position, transform.rotation);
        var projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.Initialize(projectileSpeed, 1f, false);
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

}