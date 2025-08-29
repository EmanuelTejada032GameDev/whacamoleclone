using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI damageBaseText;
    [SerializeField] private TextMeshProUGUI damageTotalText;
    [SerializeField] private TextMeshProUGUI critChanceBaseText;
    [SerializeField] private TextMeshProUGUI critChanceTotalText;
    [SerializeField] private TextMeshProUGUI critDamageBaseText;
    [SerializeField] private TextMeshProUGUI critDamageTotalText;
    [SerializeField] private TextMeshProUGUI attackSpeedBaseText;
    [SerializeField] private TextMeshProUGUI attackSpeedTotalText;

    [Header("Panel Control")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private Button toggleButton;

    [SerializeField] private bool isPanelVisible = false;

    private void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePanel);
        }

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += UpdateStatsDisplay;
        }

        UpdateStatsDisplay();

        if (statsPanel != null)
        {
            statsPanel.SetActive(isPanelVisible);
        }
    }

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += UpdateStatsDisplay;
        }
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged -= UpdateStatsDisplay;
        }
    }

    public void TogglePanel()
    {
        if (statsPanel != null)
        {
            // Get actual current state, don't trust the boolean
            bool currentState = statsPanel.activeSelf;
            bool newState = !currentState;

            statsPanel.SetActive(newState);
            isPanelVisible = newState; // Update boolean to match reality

            Debug.Log($"Panel toggled to: {newState}");
        }
    }

    private void UpdateStatsDisplay()
    {
        if (PlayerStats.Instance == null) return;

        // Damage
        if (damageBaseText != null)
            damageBaseText.text = $"{PlayerStats.Instance.BaseDamageMultiplier * 100:F0}%";
        if (damageTotalText != null)
            damageTotalText.text = $"{PlayerStats.Instance.TotalDamageMultiplier * 100:F0}%";

        // Crit Chance
        if (critChanceBaseText != null)
            critChanceBaseText.text = $"{PlayerStats.Instance.BaseCritChance:F1}%";
        if (critChanceTotalText != null)
            critChanceTotalText.text = $"{PlayerStats.Instance.TotalCritChance:F1}%";

        // Crit Damage
        if (critDamageBaseText != null)
            critDamageBaseText.text = $"{PlayerStats.Instance.BaseCritDamage:F0}%";
        if (critDamageTotalText != null)
            critDamageTotalText.text = $"{PlayerStats.Instance.TotalCritDamage:F0}%";

        // Attack Speed
        if (attackSpeedBaseText != null)
            attackSpeedBaseText.text = $"{PlayerStats.Instance.BaseAttackSpeed * 100:F0}%";
        if (attackSpeedTotalText != null)
            attackSpeedTotalText.text = $"{PlayerStats.Instance.TotalAttackSpeed * 100:F0}%";
    }

    private void Update()
    {
        // Toggle panel with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
    }
}