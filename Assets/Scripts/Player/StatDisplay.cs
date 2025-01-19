using UnityEngine;
using TMPro; // Ajout de l'import pour TextMeshPro
using System.Collections.Generic;

public class StatsDisplay : MonoBehaviour
{
    [System.Serializable]
    public class StatUI
    {
        public string attributeName;
        public TMP_Text valueText; // Changé de Text à TMP_Text
        [HideInInspector]
        public float initialValue;
        [HideInInspector]
        public bool isInitialized;
    }

    [SerializeField]
    private StatUI[] statDisplays;

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        InitializeBaseValues();
        UpdateStats();
    }

    private void InitializeBaseValues()
    {
        var attributes = player.AsReadOnlyAttributes();

        foreach (var statUI in statDisplays)
        {
            if (attributes.ContainsKey(statUI.attributeName) && !statUI.isInitialized)
            {
                var attr = attributes[statUI.attributeName];
                statUI.initialValue = attr.FinalValue();
                statUI.isInitialized = true;
            }
        }
    }

    public void UpdateStats()
    {
        var attributes = player.AsReadOnlyAttributes();

        foreach (var statUI in statDisplays)
        {
            if (attributes.ContainsKey(statUI.attributeName))
            {
                var attr = attributes[statUI.attributeName];
                float currentValue = attr.FinalValue();

                float percentage = ((currentValue - statUI.initialValue) / statUI.initialValue) * 100f;

                string sign = percentage >= 0 ? "+" : "";
                statUI.valueText.text = $"{sign}{percentage:F1}%";
            }
        }
    }
}