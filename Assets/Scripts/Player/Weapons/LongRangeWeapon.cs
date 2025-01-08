using UnityEngine;


/// <summary>
/// Class which represents a long range weapon in the game.
/// This class manage the animation of a long range weapon
/// and the attack process.
/// </summary>
public sealed class LongRangeWeapon : MonoBehaviour, IWeapons
{

    public Sprite icon;

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
    private string weaponName;

    [SerializeField]
    private ItemData.ItemType weaponType;

    [SerializeField]
    private int stackMaxCount;

    public string ItemName(){
        return weaponName;
    }

    public Sprite Icon(){
        return icon;
    }

    public ItemData.ItemType ItemType(){
        return weaponType;
    }

    public int StackMaxCount(){
        return stackMaxCount;
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
        projectile.Initialize(0.5f, 1f);
    }

}