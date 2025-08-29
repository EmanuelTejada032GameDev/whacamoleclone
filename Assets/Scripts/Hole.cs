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

    [Header("Mole Configurations")]
    [SerializeField] private MoleConfig[] moleConfigs;

    // Events
    public event Action OnMoleCompleted;
    public event Action<int> OnMoleKilled;

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
        // Destroy any existing mole to avoid nesting
        if (currentMole != null)
        {
            Destroy(currentMole.gameObject);
        }

        // Select a random configuration
        MoleConfig randomConfig = GetRandomMoleConfig();

        // Instantiate the mole model from the config
        GameObject moleObj = Instantiate(randomConfig.moleModel, moleSpawnPoint.position, Quaternion.identity, moleSpawnPoint);
        moleObj.transform.Rotate(0, 180, 0);
        currentMole = moleObj.GetComponent<Mole>();

        // Apply the configuration to the mole
        ApplyMoleConfig(currentMole, randomConfig);

        // Subscribe to mole events
        currentMole.OnMoleKilled += HandleMoleKilled;
        currentMole.OnMoleTimedOut += HandleMoleTimedOut;

        // Initialize the mole in the hidden position
        currentMole.Initialize(this, hiddenPosition, exposedPosition, moleUpTime);
    }


    private void ApplyMoleConfig(Mole mole, MoleConfig config)
    {
        // Configure health and score
        mole.GetComponent<HealthSystem>().SetMaxHealthAmount(config.maxHealth, true);
        mole.SetScoreValue(config.scoreValue);
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

    private void HandleMoleKilled(object sender, Mole.OnMoleKilledEventData e)
    {
        isOccupied = false;
        CreateMole();
        OnMoleKilled?.Invoke(e.scoreValue); 
    }


    private void HandleMoleTimedOut()
    {
        isOccupied = false;
        OnMoleCompleted?.Invoke();
    }




    private MoleConfig GetRandomMoleConfig()
    {
        return moleConfigs[UnityEngine.Random.Range(0, moleConfigs.Length)];
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