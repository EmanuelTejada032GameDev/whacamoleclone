using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private float baseCritChance = 5f; // 5% base
    [SerializeField] private float baseCritDamage = 50f; // 50% extra damage
    [SerializeField] private float baseAttackSpeed = 1f; // 1x speed multiplier

    [Header("Bonus Stats")]
    [SerializeField] private float bonusCritChance = 0f;
    [SerializeField] private float bonusCritDamage = 0f;
    [SerializeField] private float bonusAttackSpeed = 0f;

    [Header("Damage Multipliers")]
    [SerializeField] private float baseDamageMultiplier = 1f;
    [SerializeField] private float bonusDamageMultiplier = 0f;

    // Properties for easy access
    public float TotalCritChance => baseCritChance + bonusCritChance;
    public float TotalCritDamage => baseCritDamage + bonusCritDamage;
    public float TotalAttackSpeed => baseAttackSpeed + bonusAttackSpeed;
    public float TotalDamageMultiplier => baseDamageMultiplier + bonusDamageMultiplier;

    // Base values (read-only)
    public float BaseCritChance => baseCritChance;
    public float BaseCritDamage => baseCritDamage;
    public float BaseAttackSpeed => baseAttackSpeed;
    public float BaseDamageMultiplier => baseDamageMultiplier;

    // Bonus values (read-only)
    public float BonusCritChance => bonusCritChance;
    public float BonusCritDamage => bonusCritDamage;
    public float BonusAttackSpeed => bonusAttackSpeed;
    public float BonusDamageMultiplier => bonusDamageMultiplier;

    public System.Action OnStatsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCritChance(float amount)
    {
        bonusCritChance += amount;
        OnStatsChanged?.Invoke();
        Debug.Log($"Crit Chance increased by {amount}%. Total: {TotalCritChance}%");
    }

    public void AddCritDamage(float amount)
    {
        bonusCritDamage += amount;
        OnStatsChanged?.Invoke();
        Debug.Log($"Crit Damage increased by {amount}%. Total: {TotalCritDamage}%");
    }

    public void AddAttackSpeed(float amount)
    {
        bonusAttackSpeed += amount;
        OnStatsChanged?.Invoke();
        Debug.Log($"Attack Speed increased by {amount}%. Total: {TotalAttackSpeed * 100}%");
    }

    public void AddDamageMultiplier(float amount)
    {
        bonusDamageMultiplier += amount;
        OnStatsChanged?.Invoke();
        Debug.Log($"Damage increased by {amount * 100}%. Total: {TotalDamageMultiplier * 100}%");
    }

    public bool RollForCrit()
    {
        return Random.Range(0f, 100f) <= TotalCritChance;
    }

    public float CalculateDamage(float baseDamage, bool isCrit = false)
    {
        float damage = baseDamage * TotalDamageMultiplier;

        if (isCrit)
        {
            damage *= (1f + TotalCritDamage / 100f);
        }

        return damage;
    }
}