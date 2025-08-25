using UnityEngine;
using System;

public class Hole : MonoBehaviour
{
    [Header("Mole Settings")]
    [SerializeField] private GameObject molePrefab;
    [SerializeField] private Transform moleSpawnPoint;
    [SerializeField] private float moleUpTime = 2f;

    [Header("Positions")]
    [SerializeField] private Vector3 hiddenPosition = new Vector3(0, -1f, 0);
    [SerializeField] private Vector3 exposedPosition = new Vector3(0, 0, 0);

    // Events
    public event Action OnMoleCompleted;
    public event Action OnMoleKilled;

    // State
    private Mole currentMole;
    private bool isOccupied = false;

    public bool IsAvailable => !isOccupied;

    void Start()
    {
        // Create mole instance that will be reused
        CreateMole();
    }

    private void CreateMole()
    {
        GameObject moleObj = Instantiate(molePrefab, moleSpawnPoint);
        currentMole = moleObj.GetComponent<Mole>();

        // Subscribe to mole events
        currentMole.OnMoleKilled += HandleMoleKilled;
        currentMole.OnMoleTimedOut += HandleMoleTimedOut;

        // Initialize mole in hidden position
        currentMole.Initialize(this, hiddenPosition, exposedPosition, moleUpTime);
    }

    public void SpawnMoleAfterDelay(float delay)
    {
        if (isOccupied) return;

        isOccupied = true;
        Invoke(nameof(ActivateMole), delay);
    }

    private void ActivateMole()
    {
        if (currentMole == null)
        {
            CreateMole();
        }

        if (currentMole != null)
        {
            currentMole.PopUp();
        }
    }

    private void HandleMoleKilled()
    {
        isOccupied = false;
        CreateMole();
        OnMoleKilled?.Invoke();
    }

    private void HandleMoleTimedOut()
    {
        isOccupied = false;
        OnMoleCompleted?.Invoke();
    }

    void OnDestroy()
    {
        if (currentMole != null)
        {
            currentMole.OnMoleKilled -= HandleMoleKilled;
            currentMole.OnMoleTimedOut -= HandleMoleTimedOut;
        }
    }
}