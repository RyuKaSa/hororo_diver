using UnityEngine;
using UnityEngine.UI;

public class InventoryContextMenu : MonoBehaviour
{
    private int targetSlotID;

    [SerializeField] private Button dropButton, equipButton, useButton, consumeButton, closeButton;
    private Inventory inventory;

    private void Awake()
    {
        dropButton.onClick.AddListener(Drop);
        equipButton.onClick.AddListener(Equip);
        useButton.onClick.AddListener(Use);
        consumeButton.onClick.AddListener(Consume);
        closeButton.onClick.AddListener(Close);
    }

    public void Init(Inventory _inventory)
    {
        inventory = _inventory;
    }

    public void Select(int _slotID, Slot _slot)
    {
        Item _slotItem = inventory.Data[_slotID];
        ItemData _data = _slotItem.Data;

        if (_slotItem.Empty)
        {
            Close();
            return;
        }

        gameObject.SetActive(true);
        transform.position = _slot.transform.position;

        targetSlotID = _slotID;

        equipButton.gameObject.SetActive(_data is IEquipable);
        useButton.gameObject.SetActive(_data is IUsable);
        consumeButton.gameObject.SetActive(_data is IConsumable);
    }

    #region Inputs

    private void Drop()
    {
        inventory.PickItem(targetSlotID);
        Close();
    }

    private void Equip()
    {

        IEquipable _item = inventory.Data[targetSlotID].Data as IEquipable;
        _item.OnEquiped(inventory.Context);
        Close();
    }

    private void Use()
    {
        IUsable _item = inventory.Data[targetSlotID].Data as IUsable;
        _item.OnUsed(inventory.Context);
        Close();
    }

    private void Consume()
    {
        IConsumable _item = inventory.Data[targetSlotID].Data as IConsumable;
        _item.OnConsumed(inventory.Context);
        Close();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
