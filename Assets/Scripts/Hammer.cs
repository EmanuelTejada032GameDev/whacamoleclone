using UnityEngine;

public class Hammer : MonoBehaviour
{
    private Camera mainCamera;
    private float fixedYPosition;

    [Header("Hammer Stats")]
    [SerializeField] private HammerData hammerData;
    [SerializeField] private float baseDamageMultiplier = 1f;
    [SerializeField] private float baseSpeedMultiplier = 1f;

    public float BaseDamageMultiplier => baseDamageMultiplier;
    public float BaseSpeedMultiplier => baseSpeedMultiplier;

    private float lastHitTime = 0f;
    private float hitCooldown;
    private GameObject lastTargetHit;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem critParticles;
    [SerializeField] private GameObject critTextPrefab;

    void Start()
    {
        mainCamera = Camera.main;
        fixedYPosition = transform.position.y;
        UpdateHammerStats();
    }

    void Update()
    {
        FollowCursor();
        GameObject currentTarget = GetCurrentTarget();
        if (CanHit() && currentTarget != null)
        {
            DealDamageToTarget(currentTarget);
        }
    }

    private void UpdateHammerStats()
    {
        float totalSpeedMultiplier = baseSpeedMultiplier;

        if (PlayerStats.Instance != null)
        {
            totalSpeedMultiplier *= PlayerStats.Instance.TotalAttackSpeed;
        }

        hitCooldown = 1f / (hammerData.speed * totalSpeedMultiplier);
    }

    private GameObject GetCurrentTarget()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.CompareTag("Mole"))
            {
                return hitInfo.collider.gameObject;
            }
        }
        return null;
    }

    private bool CanHit()
    {
        return Time.time >= lastHitTime + hitCooldown;
    }

    private void DealDamageToTarget(GameObject target)
    {
        Mole mole = target.GetComponent<Mole>();
        if (mole != null && !mole.CanBeHit)
            return;

        float baseDamage = hammerData.damage * baseDamageMultiplier;
        bool isCrit = false;
        float finalDamage = baseDamage;

        if (PlayerStats.Instance != null)
        {
            isCrit = PlayerStats.Instance.RollForCrit();
            finalDamage = PlayerStats.Instance.CalculateDamage(baseDamage, isCrit);
        }

        target.GetComponent<HealthSystem>().TakeDamage((int)finalDamage);

        if (isCrit)
        {
            ShowCriticalHitEffects(target.transform.position);
            Debug.Log($"CRITICAL HIT! {finalDamage:F0} damage (base: {baseDamage:F0})");
        }

        if (_animator != null)
        {
            _animator.SetTrigger("hit");
        }

        lastHitTime = Time.time;
        lastTargetHit = target;
        UpdateHammerStats();
    }

    private void ShowCriticalHitEffects(Vector3 position)
    {
        if (critParticles != null)
        {
            critParticles.transform.position = position;
            critParticles.Play();
        }

        if (critTextPrefab != null)
        {
            GameObject critText = Instantiate(critTextPrefab, position, Quaternion.identity);
            Destroy(critText, 2f);
        }
    }

    private void FollowCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedYPosition, 0));
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);
            transform.position = new Vector3(targetPosition.x, fixedYPosition, (targetPosition.z - 0.1f));
        }
    }

    public void EquipNewHammer(HammerData newHammerData)
    {
        hammerData = newHammerData;
        UpdateHammerStats();
    }

    public void ApplyStatBonuses(float damageMultiplier, float speedMultiplier)
    {
        baseDamageMultiplier = damageMultiplier;
        baseSpeedMultiplier = speedMultiplier;
        UpdateHammerStats();
    }
}