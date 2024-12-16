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
    private float attackRange = 0.5f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private float attack;

    public void AttackProcessing()
    {
        Debug.Log("Player attack with " + transform.name);

        var player = GameObject.Find("Player");
        var direction = player.transform.position - transform.position;
        direction.z = 0;
        float angle = Mathf.Atan2(direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);

        // Calculate Linear equation with mob position and player position
        float gradient = (player.transform.position.y - transform.position.y) / (player.transform.position.x - transform.position.x);
        float offset = transform.position.y - (gradient * transform.position.x);


        var projectileGameObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.Initialize(0.5f, gradient, offset, 1f);

        // Detects ennemies in range of attack
        // var hitEnnemiesArray = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        // foreach (var collider in hitEnnemiesArray)
        // {
        //     var damageable = collider.GetComponent<IDamageable>();
        //     if (damageable != null)
        //     {
        //         Debug.Log("Interface was found");
        //         damageable.Damage(attack);
        //     }
        // }
    }

}