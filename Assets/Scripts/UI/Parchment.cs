using UnityEngine;

[CreateAssetMenu(menuName = "Items/Parchment Item Data")]
public class Parchment : ItemData, IUsable
{
    [SerializeField] private string text;

    void IUsable.OnUsed(InventoryContext _ctx)
    {
        Debug.Log(text);
    }
}
