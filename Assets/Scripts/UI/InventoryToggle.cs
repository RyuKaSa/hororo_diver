using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // Faites glisser votre panel d'inventaire ici dans l'inspecteur
    private bool isInventoryOpen = false;

    void Update()
    {
        // V�rifie si la touche A est press�e
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Bascule la visibilit� du panel
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        // Inverse l'�tat actuel de l'inventaire
        isInventoryOpen = !isInventoryOpen;

        // Applique la visibilit� du panel en fonction de l'�tat
        inventoryPanel.SetActive(isInventoryOpen);
    }
}