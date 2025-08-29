using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RewardCard : MonoBehaviour
{
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardDescriptionText;
    [SerializeField] private Button takeButton;
    [SerializeField] private Button declineButton;

    private RewardData currentReward;

    public void ShowReward(RewardData reward)
    {
        currentReward = reward;
        rewardIcon.sprite = reward.icon;
        rewardDescriptionText.SetText(reward.description);

        // Show the reward card UI
        gameObject.SetActive(true);

        // Add listeners to the buttons
        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(ApplyReward);

        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(DeclineReward);
    }

    private void ApplyReward()
    {
        // Apply the reward
        GameManager.Instance.ApplyReward(currentReward);

        // Hide the reward card UI
        gameObject.SetActive(false);
    }

    private void DeclineReward()
    {
        // Hide the reward card UI without applying the reward
        gameObject.SetActive(false);

        // Optionally, you can log or handle the declined reward here
        Debug.Log("Reward declined by the player.");
    }
}