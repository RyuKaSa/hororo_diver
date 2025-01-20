using UnityEngine;


/// <summary>
/// Class which represents an Artifact in the game.
/// This class is associated to a Box Collider 2D
/// in a GameObject to detects if Player touch it
/// </summary>
public sealed class Artifact : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private string name;

    [SerializeField]
    private Inventory inventory;

    void Start()
    {
        var go = GameObject.Find("Inventory UI");
        if (go == null)
        {
            Debug.Log("Inventory UI not found");
            go = GameObject.Find("Player");
            if (go == null)
            {
                Debug.Log("Player inventory not found");
            }
        }
        inventory = go.GetComponent<Inventory>();
    }

    public string Name()
    {
        return name;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION !");
        // Check collision with Player Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // inventory.AddItem(FromArtifactName());
            var component = GameObject.Find("MisterFish").GetComponent<MisterFish>();
            component.TriggerKinematic();
            Destroy(gameObject);
        }
    }

    public Item FromArtifactName()
    {
        var itemDB = Utils.GetComponentFromGameObjectTag<ItemDatabase>("ItemDatabase");
        if (itemDB == null)
        {
            Debug.Log("Error: Could not find ItemDatabase component");
        }

        var artifact = itemDB.GetItemByName(name);
        if (artifact == null)
        {
            Debug.Log("Error: Could not find ore which called " + name);
        }
        return new Item(artifact, 1);

    }



}