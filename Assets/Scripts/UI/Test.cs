using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Item itemToPush, pickedItem;

    private Inventory inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        Add();
    }

    [ContextMenu("Test push")]
    private void Add()
    {
        itemToPush = inventory.AddItem(itemToPush);
    }

    [ContextMenu("Test pick")]
    private void Pick()
    {
        pickedItem = inventory.PickItem(1);
    }
}
