using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// Class which represents an Ore in the game.
/// This class is associated to a Box Collider 2D
/// in a GameObject to detects if Player touch
/// the ore with his pickaxe
/// </summary>
public sealed class Ore : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private int hitRequired; // Hit required to get ore

    [SerializeField]
    private string name;

    [SerializeField]
    private ColoredFlash coloredFlash;

    private int currentHit = 0;


    /// <summary>
    /// This function add hit to the ore and
    /// Checks if Player has touched the ore 
    ///enough times to recover it.
    /// </summary>
    /// <return> true if the Player can get it false otherwise </return>
    public bool HitOre()
    {
        currentHit += 1;
        Debug.Log("Current hit = " + currentHit + " required == current = " + (currentHit == hitRequired));
        coloredFlash.Flash(Color.white);
        var audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        return currentHit == hitRequired;
    }

    public string Name()
    {
        return name;
    }

    void Update()
    {
        if (currentHit == hitRequired)
        {
            Debug.Log("DESTROY ORE " + gameObject.name);
            DestroyInMap();
        }
    }

    private void DestroyInMap()
    {
        GridLayout gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        if (gridLayout == null)
        {
            Debug.Log("GridLayout is null");
            return;
        }
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);

        var tilemap = transform.parent.GetComponentInParent<Tilemap>();
        tilemap.SetTile(cellPosition, null);
    }

}