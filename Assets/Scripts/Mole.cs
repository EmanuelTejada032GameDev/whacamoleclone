using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Mole : MonoBehaviour
{
    [Header("Animation Settings")]
    public float popUpHeight = 1f;
    public float animationSpeed = 2f;

    [Header("Architecture Settings")]
    [SerializeField] private float moleUpTime = 2f;

    // Animation state
    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;
    private bool isVisible = false;
    public bool IsVisible => isVisible;
    [SerializeField] private HealthSystem _healthSystem;

    // Architecture events (needed by Hole script)
    public event Action OnMoleKilled;
    public event Action OnMoleTimedOut;

    // Architecture state
    private Hole parentHole;
    private bool isActive = false;
    private Coroutine upSequenceCoroutine;

    [Header("Death Effects")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    [Header("Hit Detection")]
    [SerializeField] private float hitThreshold = 0.05f;

    public bool CanBeHit => isActive && isVisible &&
                        Vector3.Distance(transform.position, visiblePosition) <= hitThreshold;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        // Setup positions
        hiddenPosition = transform.position;
        visiblePosition = hiddenPosition + Vector3.up * popUpHeight;

        // Subscribe to health system
        _healthSystem.OnDied += OnDied;
    }

    private void OnDied(object sender, EventArgs e)
    {
        HandleDeath();
    }

    // Your original Update - animation system
    private void Update()
    {
        Vector3 targetPos = isVisible ? visiblePosition : hiddenPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, animationSpeed * Time.deltaTime);
    }

    // Public methods for manual control (if needed)
    public void ShowMole()
    {
        isVisible = true;
        if (!isActive)
        {
            isActive = true;
        }
    }

    public void HideMole()
    {
        isVisible = false;
        if (isActive)
        {
            isActive = false;
            OnMoleTimedOut?.Invoke();
        }
    }

    // ============================================================================
    // ARCHITECTURE METHODS (needed by Hole script)
    // ============================================================================

    public void Initialize(Hole hole, Vector3 hiddenPos, Vector3 exposedPos, float timeUp)
    {
        parentHole = hole;
        moleUpTime = timeUp;

        // Convert relative positions to world positions
        hiddenPosition = transform.parent.TransformPoint(hiddenPos);
        visiblePosition = transform.parent.TransformPoint(exposedPos);

        // Start in hidden position
        transform.position = hiddenPosition;
        isVisible = false;
        isActive = false;
    }

    public void PopUp()
    {
        if (isActive) return;

        // Reset health for reuse
        if (_healthSystem != null)
        {
            _healthSystem.ResetHealth();
        }

        isActive = true;
        upSequenceCoroutine = StartCoroutine(PopUpSequence());
    }

    private IEnumerator PopUpSequence()
    {
        // Pop up using your original animation system
        isVisible = true;

        // Wait for mole to reach visible position
        while (Vector3.Distance(transform.position, visiblePosition) > 0.01f)
        {
            yield return null;
        }

        // Stay up for specified time
        yield return new WaitForSeconds(moleUpTime);

        // Pop down if still active (not killed)
        if (isActive)
        {
            isVisible = false;

            // Wait for mole to reach hidden position
            while (Vector3.Distance(transform.position, hiddenPosition) > 0.01f)
            {
                yield return null;
            }

            isActive = false;
            OnMoleTimedOut?.Invoke();
        }
    }

    private void HandleDeath()
    {
        if (!isActive) return;

        isActive = false;

        // Stop any running coroutines
        if (upSequenceCoroutine != null)
        {
            StopCoroutine(upSequenceCoroutine);
            upSequenceCoroutine = null;
        }

        // Play death effects
        if (deathParticles != null)
            deathParticles.Play();

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Notify hole immediately
        OnMoleKilled?.Invoke();

        // Destroy after brief delay to allow effects to play
        Destroy(gameObject, 0.2f);
    }


    private IEnumerator WaitForHideAndNotify()
    {
        // Wait for your animation to hide the mole
        while (Vector3.Distance(transform.position, hiddenPosition) > 0.01f)
        {
            yield return null;
        }

        OnMoleKilled?.Invoke();
    }

    private void OnDestroy()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnDied -= OnDied;
        }
    }
}