using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Item itemToPush, pickedItem;

    [SerializeField] private OreData oreData;

    private Inventory inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        Add();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddIron();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCobalt();
        }

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

    private void AddIron()
    {
        Debug.Log("Add Iron !");
        var iron = new Item(oreData, 1);
        inventory.AddItem(iron);
    }

    private void AddCobalt()
    {
        Debug.Log("Add Cobalt !");
        var cobalt = new Item(oreData, 1);
        inventory.AddItem(cobalt);
    }

}
