using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // Faites glisser votre panel d'inventaire ici dans l'inspecteur
    private bool isInventoryOpen = false;

    void Update()
    {
        // Vérifie si la touche A est pressée
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Bascule la visibilité du panel
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        // Inverse l'état actuel de l'inventaire
        isInventoryOpen = !isInventoryOpen;

        // Applique la visibilité du panel en fonction de l'état
        inventoryPanel.SetActive(isInventoryOpen);
    }
}