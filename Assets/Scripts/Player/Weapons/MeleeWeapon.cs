using UnityEngine;


/// <summary>
/// Class which represents a melee weapon in the game.
/// This class manage the animation of a melee weapon
/// and the attack process.
/// </summary>
public sealed class MeleeWeapon : MonoBehaviour, IWeapons
{
    public Animator animator;

    public Sprite icon;

    [SerializeField]
    private float attackRange = 0.5f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private float attack;

    [SerializeField]
    private string weaponName;

    [SerializeField]
    private ItemData.ItemType weaponType;

    [SerializeField]
    private int stackMaxCount;

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

}