using UnityEngine;

[CreateAssetMenu(menuName ="Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] public string itemName;

    [SerializeField] public int stackMaxCount;

    [SerializeField] public Sprite icon;
}
