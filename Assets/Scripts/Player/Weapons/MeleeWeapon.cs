using UnityEngine;


/// <summary>
/// Class which represents a melee weapon in the game.
/// This class manage the animation of a melee weapon
/// and the attack process.
/// </summary>
public sealed class MeleeWeapon : MonoBehaviour, IWeapons
{
    public Animator animator;

    [SerializeField]
    private float attackRange = 0.5f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private float attack;

    [SerializeField]
    private string weaponName;

    public string WeaponName()
    {
        return weaponName;
    }

    public void AttackProcessing()
    {
        Debug.Log("Player attack with " + transform.name);
        // Detects ennemies in range of attack
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
        Debug.Log("Info: equip " + transform.name);
        _ctx.EquipWeapon(this);
    }


}