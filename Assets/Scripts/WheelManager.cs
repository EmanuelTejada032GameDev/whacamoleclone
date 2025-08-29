using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    [Header("Wheel Settings")]
    [SerializeField] private List<RewardData> levelRewards;
    [SerializeField] private Transform wheelTransform;
    [SerializeField] private GameObject rewardSectionPrefab;
    [SerializeField] private float minSpinDuration = 2f;
    [SerializeField] private float maxSpinDuration = 4f;
    [SerializeField] private AnimationCurve spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("UI References")]
    [SerializeField] private GameObject wheelUI;
    [SerializeField] private Button spinButton;
    [SerializeField] private RectTransform wheelSelector;

    private RewardData selectedReward;
    [SerializeField] private RewardCard rewardCard;

    public System.Action<RewardData> OnRewardSelected;

    private bool isSpinning = false;
    private List<RewardSection> currentRewardSections = new List<RewardSection>();

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        spinButton.onClick.AddListener(SpinWheel);
    }

    public void ShowWheel(List<RewardData> rewards)
    {
        levelRewards = rewards;
        wheelUI.SetActive(true);
        PopulateWheel(rewards);
    }

    private void SpinWheel()
    {
        spinButton.interactable = false;
        isSpinning = true;
        StartCoroutine(SpinWheelCoroutine());
    }

    private IEnumerator SpinWheelCoroutine()
    {
        float spinDuration = Random.Range(minSpinDuration, maxSpinDuration);
        float elapsed = 0f;
        float startAngle = wheelTransform.eulerAngles.z;
        float totalRotation = Random.Range(1080f, 2160f); // 3-6 full rotations
        float endAngle = startAngle + totalRotation;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            float curveValue = spinCurve.Evaluate(t);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, curveValue);
            wheelTransform.eulerAngles = new Vector3(0, 0, currentAngle);
            yield return null;
        }

        wheelTransform.eulerAngles = new Vector3(0, 0, endAngle);
        yield return new WaitForSeconds(0.5f);

        isSpinning = false;
        CheckSelectedRewardByAngle();

        yield return new WaitForSeconds(2f);

        if (selectedReward != null)
        {
            spinButton.interactable = true;
            rewardCard.ShowReward(selectedReward);
        }

        wheelUI.SetActive(false);
    }

    public void PopulateWheel(List<RewardData> rewards)
    {
        foreach (Transform child in wheelTransform)
        {
            if (child.CompareTag("RewardSection"))
            {
                Destroy(child.gameObject);
            }
        }

        currentRewardSections.Clear();

        int rewardCount = rewards.Count;
        float angleStep = 360f / rewardCount;

        for (int i = 0; i < rewardCount; i++)
        {
            GameObject section = Instantiate(rewardSectionPrefab, wheelTransform);
            section.transform.localRotation = Quaternion.Euler(0, 0, -angleStep * i);

            RewardSection rewardSection = section.GetComponent<RewardSection>();
            rewardSection.SetReward(rewards[i]);

            currentRewardSections.Add(rewardSection);
            section.tag = "RewardSection";
        }
    }

    private void CheckSelectedRewardByAngle()
    {
        if (currentRewardSections.Count == 0) return;

        float wheelRotation = wheelTransform.eulerAngles.z;
        float anglePerSection = 360f / currentRewardSections.Count;

        wheelRotation = wheelRotation % 360f;
        if (wheelRotation < 0) wheelRotation += 360f;

        float selectorAngle = (360f - wheelRotation) % 360f;
        int selectedSectionIndex = Mathf.FloorToInt(selectorAngle / anglePerSection);

        selectedSectionIndex = selectedSectionIndex % currentRewardSections.Count;
        if (selectedSectionIndex < 0) selectedSectionIndex += currentRewardSections.Count;

        if (showDebugLogs)
        {
            Debug.Log($"Wheel Rotation: {wheelRotation:F1}° | Selector Angle: {selectorAngle:F1}° | Selected Section: {selectedSectionIndex}");
        }

        if (selectedSectionIndex >= 0 && selectedSectionIndex < currentRewardSections.Count)
        {
            RewardSection selectedSection = currentRewardSections[selectedSectionIndex];
            if (selectedSection != null)
            {
                selectedReward = selectedSection.GetReward();
                if (showDebugLogs)
                {
                    Debug.Log($"Selected Reward: {selectedReward.rewardType}");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || wheelTransform == null) return;

        if (currentRewardSections.Count > 0)
        {
            float wheelRotation = wheelTransform.eulerAngles.z;
            float anglePerSection = 360f / currentRewardSections.Count;
            float selectorAngle = (360f - wheelRotation) % 360f;
            int selectedSectionIndex = Mathf.FloorToInt(selectorAngle / anglePerSection);
            selectedSectionIndex = selectedSectionIndex % currentRewardSections.Count;

            Vector3 wheelCenter = wheelTransform.position;
            float radius = 2f;

            // Draw selector line (pointing up from wheel center)
            Gizmos.color = Color.red;
            Vector3 selectorDirection = Vector3.up;
            Gizmos.DrawLine(wheelCenter, wheelCenter + selectorDirection * radius);

            // Draw section boundaries
            Gizmos.color = Color.yellow;
            for (int i = 0; i < currentRewardSections.Count; i++)
            {
                float sectionAngle = -i * anglePerSection + wheelRotation;
                Vector3 direction = new Vector3(
                    Mathf.Sin(sectionAngle * Mathf.Deg2Rad),
                    Mathf.Cos(sectionAngle * Mathf.Deg2Rad),
                    0
                );
                Gizmos.DrawLine(wheelCenter, wheelCenter + direction * radius * 0.8f);
            }

            // Highlight selected section
            if (selectedSectionIndex >= 0 && selectedSectionIndex < currentRewardSections.Count)
            {
                Gizmos.color = Color.green;
                float selectedAngle = -selectedSectionIndex * anglePerSection + wheelRotation;
                Vector3 selectedDirection = new Vector3(
                    Mathf.Sin(selectedAngle * Mathf.Deg2Rad),
                    Mathf.Cos(selectedAngle * Mathf.Deg2Rad),
                    0
                );
                Gizmos.DrawLine(wheelCenter, wheelCenter + selectedDirection * radius * 1.2f);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isSpinning)
        {
            CheckSelectedRewardByAngle();
        }
    }
}