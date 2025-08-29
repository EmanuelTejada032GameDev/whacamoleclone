using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int maxActiveMoles = 3;
    [SerializeField] private Hole[] holes;
    private int currentActiveMoles = 0;
    private int score = 0;


    [Header("Level System Config")]
    [SerializeField] private List<int> levelThresholds = new List<int>(){ 200, 400, 800, 1600, 3800 , 6800, 11000 , 15000, 21000 }; 
    private int currentLevel = 0;

    [Header("Level Configurations")]
    [SerializeField] private List<LevelConfigSO> levelConfigs;


    [Header("LVL UI")]
    [SerializeField] private Image levelProgressBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Wheel Manager Config")]
    [SerializeField] private WheelManager wheelManager;
    [SerializeField] private List<RewardData> allRewards; // All possible rewards
    [SerializeField] private List<RewardData> levelRewards;


    public System.EventHandler<ScoreChangeEventData> onScoreChanged;


    [SerializeField] private Hammer playerHammer; // Reference to the player's hammer
    [SerializeField] private GameObject hammer; 



    public class ScoreChangeEventData
    {
        public int scoreIncrease;
    }

    private void Awake()
    {
        Instance = this;
        scoreText.SetText("0");
        UpdateLevelProgressBar();
    }

    void Start()
    {
        // Register to hole events
        foreach (var hole in holes)
        {
            hole.OnMoleCompleted += HandleMoleCompleted;
            hole.OnMoleKilled += scoreValue => HandleMoleKilled(scoreValue);
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

    private void HandleMoleKilled(int scoreValue)
    {
        currentActiveMoles--;
        score += scoreValue; // Use the actual score value from the MoleConfig
        scoreText.SetText(score.ToString());
        onScoreChanged?.Invoke(this, new ScoreChangeEventData { scoreIncrease = scoreValue });

        UpdateLevelProgressBar();
    }


    private Coroutine progressBarCoroutine;


    private void UpdateLevelProgressBar()
    {
        if (currentLevel >= levelThresholds.Count)
        {
            StartSmoothFill(1f);
            return;
        }

        int currentLevelThreshold = levelThresholds[currentLevel];
        int previousLevelThreshold = currentLevel > 0 ? levelThresholds[currentLevel - 1] : 0;

        if (score >= currentLevelThreshold)
        {
            currentLevel++;
            levelText.SetText($"lvl {currentLevel}");

            // Pause mole spawning
            CancelInvoke(nameof(TrySpawnMole));

            // Disable the hammer
            hammer.SetActive(false);

            // Show the wheel
            levelRewards = GetLevelRewards(currentLevel);
            wheelManager.ShowWheel(levelRewards);
            wheelManager.OnRewardSelected += ApplyReward;

            UpdateLevelProgressBar();
            return;
        }

        float progress = (float)(score - previousLevelThreshold) / (currentLevelThreshold - previousLevelThreshold);
        StartSmoothFill(progress);
    }


    private List<RewardData> GetLevelRewards(int level)
    {
        // Find the LevelConfigSO for the given level
        LevelConfigSO levelConfig = levelConfigs.FirstOrDefault(config => config.Level == level);

        if (levelConfig != null)
        {
            // Return the possible rewards for the level
            return levelConfig.PosibleRewards.ToList();
        }

        Debug.LogError($"No LevelConfigSO found for level {level}");
        return new List<RewardData>();
    }


    public void ApplyReward(RewardData reward)
    {
        switch (reward.rewardType)
        {
            case RewardData.RewardType.DamageIncrease:
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddDamageMultiplier(reward.value / 100f);
                }
                break;

            case RewardData.RewardType.HitSpeedIncrease:
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddAttackSpeed(reward.value / 100f);
                }
                break;

            case RewardData.RewardType.CritDamageIncrease:
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddCritDamage(reward.value);
                }
                break;

            case RewardData.RewardType.CritChanceIncrease:
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddCritChance(reward.value);
                }
                break;

            case RewardData.RewardType.NewHammer:
                playerHammer.EquipNewHammer(reward.newHammer);
                break;
        }

        // Resume mole spawning
        hammer.SetActive(true);
        StartSpawning();
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