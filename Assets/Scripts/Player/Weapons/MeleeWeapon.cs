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

}