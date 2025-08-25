using UnityEngine;

public class Hammer : MonoBehaviour
{
    private Camera mainCamera;
    private float fixedYPosition;
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitRate = 1f;
    [SerializeField] private bool resetCooldownOnTargetExit = false;

    private float lastHitTime = 0f;
    private float hitCooldown;
    private GameObject lastTargetHit;
    private bool wasOverTarget = false;
    [SerializeField] private Animator _animator;

    void Start()
    {
        mainCamera = Camera.main;
        fixedYPosition = transform.position.y;
        hitCooldown = 1f / hitRate;
    }

    void Update()
    {
        FollowCursor();
        GameObject currentTarget = GetCurrentTarget();

        if (resetCooldownOnTargetExit)
        {
            HandleTargetTransition(currentTarget);
        }

        if (CanHit() && currentTarget != null)
        {
            DealDamageToTarget(currentTarget);
        }
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

    private void HandleTargetTransition(GameObject currentTarget)
    {
        bool isOverTarget = currentTarget != null;

        if (isOverTarget && !wasOverTarget)
        {
            lastHitTime = 0f;
        }

        wasOverTarget = isOverTarget;
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

        target.GetComponent<HealthSystem>().TakeDamage(damage);
        if (_animator != null)
        {
            _animator.SetTrigger("hit");
        }
        lastHitTime = Time.time;
        lastTargetHit = target;
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
}