using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private int index;
    private Vector3 initialImageLocalPosition;

    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Image itemImage;
    private InventoryDisplay inventoryDisplay;
    private Button button;

    public void Initialize(InventoryDisplay _inventoryDisplay, int _index)
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
        index = _index;
        inventoryDisplay = _inventoryDisplay;
    }

    public void UpdateDisplay(Item _item)
    {
        if (!_item.Empty)
        {
            itemCountText.text = _item.Count.ToString();
            itemImage.sprite = _item.Data.icon;
            itemImage.color = Color.white;
            return;
        }

        itemCountText.text = "";
        itemImage.sprite = null;
        itemImage.color = new Color(0, 0, 0, 0);
    }

    private void OnClick()
    {
        inventoryDisplay.ClickSlot(index);
    }

    #region Drag and Drop
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        inventoryDisplay.DragSlot(index);

        initialImageLocalPosition = itemImage.transform.localPosition;
        itemImage.transform.SetParent(inventoryDisplay.transform);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        itemImage.transform.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        itemImage.transform.SetParent(transform);
        itemImage.transform.localPosition = initialImageLocalPosition;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        inventoryDisplay.DropOnSlot(index);
    }

    #endregion
}
