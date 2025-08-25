using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;
    public event EventHandler OnDied;

    private int _healthAmount;
    [SerializeField] private int _maxHealthAmount;

    public void Awake()
    {
        _healthAmount = _maxHealthAmount;
    }

    public int HealthAmount => _healthAmount;
    public int MaxHealth => _maxHealthAmount;
    public float NormalizedHealthAmount => (float)_healthAmount / _maxHealthAmount;

    public void TakeDamage(int damageAmount)
    {
        _healthAmount -= damageAmount;
        _healthAmount = Mathf.Clamp(_healthAmount, 0, _maxHealthAmount); 
        OnDamaged?.Invoke(this, EventArgs.Empty);
        if (IsDead()) OnDied?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int amount)
    {
        _healthAmount += amount;
        _healthAmount = Mathf.Clamp(_healthAmount, 0, _maxHealthAmount);
        OnHealed?.Invoke(this, EventArgs.Empty);
    }

    public void HealFull()
    {
        _healthAmount = _maxHealthAmount; 
        OnHealed?.Invoke(this, EventArgs.Empty);
    }

    public void ResetHealth()
    {
        _healthAmount = _maxHealthAmount;
        OnHealed?.Invoke(this, EventArgs.Empty);
    }

    public bool IsDead() => _healthAmount == 0;
    public bool IsFullHealth() => _healthAmount == _maxHealthAmount;

    public void SetMaxHealthAmount(int maxHealthAmount, bool setHealthAmount = false)
    {
        _maxHealthAmount = maxHealthAmount;
        if (setHealthAmount) _healthAmount = maxHealthAmount;
    }
}