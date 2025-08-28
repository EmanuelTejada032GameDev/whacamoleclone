// 2025-08-28 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int maxActiveMoles = 3;

    [Header("Level System")]
    [SerializeField] private List<int> levelThresholds = new List<int>(){ 200, 400, 800, 1600, 3800, 4500 , 6800, 11000 , 15000, 21000 }; 
    private int currentLevel = 0;

    [Header("References")]
    [SerializeField] private Hole[] holes;
    [SerializeField] private Image levelProgressBar; // Reference to the bar
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;



    private int currentActiveMoles = 0;
    private int score = 0;

    public System.EventHandler<ScoreChangeEventData> onScoreChanged;

    public class ScoreChangeEventData
    {
        public int scoreIncrease;
    }

    private void Awake()
    {
        scoreText.SetText("0");
        UpdateLevelProgressBar();
    }

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
            float delay = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            availableHole.SpawnMoleAfterDelay(delay);
            currentActiveMoles++;
        }
    }

    private Hole GetRandomAvailableHole()
    {
        var availableHoles = System.Array.FindAll(holes, h => h.IsAvailable);
        return availableHoles.Length > 0 ? availableHoles[UnityEngine.Random.Range(0, availableHoles.Length)] : null;
    }

    private void HandleMoleCompleted()
    {
        currentActiveMoles--;
        // Mole went down naturally - no score
    }

    private void HandleMoleKilled()
    {
        currentActiveMoles--;
        int scoreIncrease = 10; // Add score for successful hit
        score += scoreIncrease;
        scoreText.SetText(score.ToString());
        onScoreChanged?.Invoke(this, new ScoreChangeEventData { scoreIncrease = scoreIncrease });

        UpdateLevelProgressBar();
    }
    private Coroutine progressBarCoroutine;

    private void UpdateLevelProgressBar()
    {
        if (currentLevel >= levelThresholds.Count)
        {
            // Max level reached
            StartSmoothFill(1f);
            return;
        }

        int currentLevelThreshold = levelThresholds[currentLevel];
        int previousLevelThreshold = currentLevel > 0 ? levelThresholds[currentLevel - 1] : 0;

        // Check if the player has leveled up
        if (score >= currentLevelThreshold)
        {
            currentLevel++;
            levelText.SetText($"lvl {currentLevel}");
            UpdateLevelProgressBar(); // Recalculate for the new level
            return;
        }

        // Calculate progress within the current level
        float progress = (float)(score - previousLevelThreshold) / (currentLevelThreshold - previousLevelThreshold);
        StartSmoothFill(progress);
    }

    private void StartSmoothFill(float targetFillAmount)
    {
        if (progressBarCoroutine != null)
        {
            StopCoroutine(progressBarCoroutine);
        }
        progressBarCoroutine = StartCoroutine(SmoothFillCoroutine(targetFillAmount));
    }

    private IEnumerator SmoothFillCoroutine(float targetFillAmount)
    {
        float startFillAmount = levelProgressBar.fillAmount;
        float duration = 0.5f; // Duration of the smooth transition
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            levelProgressBar.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsed / duration);
            yield return null;
        }

        levelProgressBar.fillAmount = targetFillAmount;
    }
}