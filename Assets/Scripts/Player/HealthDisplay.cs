using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [System.Serializable]
    public class HealthIcon
    {
        public Image healthImage;
    }

    [SerializeField]
    private HealthIcon[] healthIcons;

    [SerializeField]
    private Sprite fullHeartSprite;

    [SerializeField]
    private Sprite emptyHeartSprite;

    [SerializeField]
    private int maxHealth = 3; // On le rend configurable dans l'inspecteur

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Vérifier que nous avons assez d'icônes
        if (healthIcons.Length < maxHealth)
        {
            Debug.LogWarning($"Not enough health icons! Have {healthIcons.Length}, need {maxHealth}");
        }

        player.OnHealthChanged += UpdateHealthDisplay;
        UpdateHealthDisplay();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthDisplay;
        }
    }

    public void UpdateHealthDisplay()
    {
        float currentHealth = player.GetHealth();

        // Limiter l'affichage au maximum de vies configuré
        int iconCount = Mathf.Min(healthIcons.Length, maxHealth);

        for (int i = 0; i < iconCount; i++)
        {
            if (healthIcons[i].healthImage != null)
            {
                healthIcons[i].healthImage.sprite = i < currentHealth ? fullHeartSprite : emptyHeartSprite;
            }
        }

        // Désactiver les icônes en surplus si on en a trop
        for (int i = iconCount; i < healthIcons.Length; i++)
        {
            if (healthIcons[i].healthImage != null)
            {
                healthIcons[i].healthImage.gameObject.SetActive(false);
            }
        }
    }
}