using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int maxActiveMoles = 3;

    [Header("References")]
    [SerializeField] private Hole[] holes;

    private int currentActiveMoles = 0;
    private int score = 0;

    void Start()
    {
        // Register to hole events
        foreach (var hole in holes)
        {
            hole.OnMoleCompleted += HandleMoleCompleted;
            hole.OnMoleKilled += HandleMoleKilled;
        }

        StartSpawning();
    }

    private void StartSpawning()
    {
        InvokeRepeating(nameof(TrySpawnMole), 1f, 0.5f);
    }

    private void TrySpawnMole()
    {
        if (currentActiveMoles >= maxActiveMoles) return;

        // Find available holes
        Hole availableHole = GetRandomAvailableHole();
        if (availableHole != null)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            availableHole.SpawnMoleAfterDelay(delay);
            currentActiveMoles++;
        }
    }

    private Hole GetRandomAvailableHole()
    {
        var availableHoles = System.Array.FindAll(holes, h => h.IsAvailable);
        return availableHoles.Length > 0 ? availableHoles[Random.Range(0, availableHoles.Length)] : null;
    }

    private void HandleMoleCompleted()
    {
        currentActiveMoles--;
        // Mole went down naturally - no score
    }

    private void HandleMoleKilled()
    {
        currentActiveMoles--;
        score += 10; // Add score for successful hit
        Debug.Log($"Score: {score}");
    }



}
