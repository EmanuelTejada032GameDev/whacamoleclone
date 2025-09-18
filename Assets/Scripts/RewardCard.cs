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

        gameObject.SetActive(true);

        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(ApplyReward);

        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(DeclineReward);
    }

    private void ApplyReward()
    {
        GameManager.Instance.ApplyReward(currentReward);

        gameObject.SetActive(false);
    }

    private void DeclineReward()
    {
        gameObject.SetActive(false);

        Debug.Log("Reward declined by the player.");
    }
}